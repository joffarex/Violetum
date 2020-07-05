﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Joffarex.Specification;
using Microsoft.EntityFrameworkCore;
using Violetum.Domain.CustomExceptions;
using Violetum.Domain.Entities;
using Violetum.Domain.Infrastructure;

namespace Violetum.Infrastructure.Repositories
{
    public class CommunityRepository : BaseRepository, IAsyncRepository<Community>
    {
        private readonly ApplicationDbContext _context;

        public CommunityRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<TResult> GetByConditionAsync<TResult>(Expression<Func<TResult, bool>> condition,
            IConfigurationProvider configurationProvider) where TResult : class
        {
            return await _context.Communities
                .Include(x => x.Author)
                .ProjectTo<TResult>(configurationProvider)
                .Where(condition)
                .FirstOrDefaultAsync();
        }

        public async Task<Community> GetByConditionAsync(Expression<Func<Community, bool>> condition)
        {
            return await _context.Communities.Include(x => x.Author).Where(condition).FirstOrDefaultAsync();
        }

        public async Task<IReadOnlyList<TResult>> ListAsync<TResult>(ISpecification<Community> specification,
            IConfigurationProvider configurationProvider) where TResult : class
        {
            IQueryable<TResult> query =
                await ApplySpecification<Community, TResult>(specification, configurationProvider);

            return await query.ToListAsync();
        }

        public async Task<int> GetTotalCountAsync(ISpecification<Community> specification)
        {
            IQueryable<Community> query = await ApplySpecification(specification);

            return await query.CountAsync();
        }

        public async Task CreateAsync(Community community)
        {
            await _context.Communities.AddAsync(community);

            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Community community)
        {
            _context.Communities.Update(community);

            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Community community)
        {
            List<Post> posts = await _context.Posts.Where(x => x.CommunityId == community.Id).ToListAsync();
            if (posts.Any())
            {
                throw new HttpStatusCodeException(HttpStatusCode.UnprocessableEntity,
                    "Can not delete category while there are still posts in it");
            }

            _context.Communities.Remove(community);

            await _context.SaveChangesAsync();
        }
    }
}