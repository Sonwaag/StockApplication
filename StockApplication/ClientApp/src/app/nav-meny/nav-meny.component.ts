import { Component } from '@angular/core'
import { clientUser } from '../TsClasses';
@Component({
    selector: 'app-nav-meny',
    templateUrl: './nav-meny.component.html',
    styleUrls: ['./nav-meny.component.css']
})

export class NavMenuComponent {
    public signtext: string;
    txt1: string = "Sign-in";
    txt2: string = "Sign-out";
    
    ngOnInit() {
        this.setLogin();
    }
    setLogin() {
        this.signtext = this.txt1;
    }
    setLogout() {
        this.signtext = this.txt2;
    }

}