import {
  Component,
  OnInit,
  Renderer2,
  HostListener,
  Inject, OnDestroy
} from '@angular/core';
import {Location} from '@angular/common';
import {DOCUMENT} from '@angular/common';
import {OidcSecurityService} from 'angular-auth-oidc-client';
import {HttpClient, HttpHeaders} from '@angular/common/http';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent implements OnInit, OnDestroy {
  userId: string;
  isCollapsed = true;

  constructor(
    private renderer: Renderer2,
    public location: Location,
    @Inject(DOCUMENT) private document: Document,
    public oidcSecurityService: OidcSecurityService,
    public http: HttpClient,
  ) {
  }

  @HostListener('window:scroll', ['$event'])
  onWindowScroll(e) {
    const element = document.getElementById('navbar-top');

    if (window.pageYOffset > 100) {
      if (element) {
        element.classList.remove('navbar-transparent');
        element.classList.add('bg-primary');
      }
    } else {
      if (element) {
        element.classList.add('navbar-transparent');
        element.classList.remove('bg-primary');
      }
    }
  }

  ngOnInit() {
    this.onWindowScroll(event);
    this.oidcSecurityService
      .checkAuth()
      .subscribe((auth) => {
        console.log('is authenticated', auth);
        if (auth) {
          this.userId = this.oidcSecurityService.getPayloadFromIdToken().sub;
          console.log(this.userId);
        }
      });
  }

  login() {
    this.oidcSecurityService.authorize();
  }

  register() {
    this.document.location.href = `http://localhost:5000/auth/register?returnUrl=${window.location.href}`;
  }

  callApi() {
    const token = this.oidcSecurityService.getToken();

    this.http.get('http://localhost:5001/api/v1/posts?OrderByDir=desc&SortBy=VoteCount&CurrentPage=1', {
      headers: new HttpHeaders({
        Authorization: 'Bearer ' + token,
      }),
      responseType: 'text',
    })
      .subscribe((data: any) => {
        console.log(JSON.parse(data));

      });
  }

  ngOnDestroy() {
    const body = document.getElementsByTagName('body')[0];
    body.classList.remove('index-page');
  }
}