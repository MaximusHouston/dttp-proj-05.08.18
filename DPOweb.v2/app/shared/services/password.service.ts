import { Injectable } from '@angular/core';


@Injectable()
export class PasswordService {

    PasswordStrengthLevel(password: string) {
        var strength = 0;
        if (this.ContainsLowerCase(password)) {
            strength++;
        }
        if (this.ContainsUpperCase(password)) {
            strength++;
        }
        if (this.ContainsNumber(password)) {
            strength++;
        }
        if (this.ContainsSpecialCharacter(password)) {
            strength++;
        } if (password.length >= 8) {
            strength++;
        }
        return strength;
    }   

    ContainsLowerCase(password: string) {
        var patt = /[a-z]/g;
        var result = password.match(patt);
        if (result != null && result.length > 0) {
            return true;
        } else return false;
    }

    ContainsUpperCase(password: string) {
        var patt = /[A-Z]/g;
        var result = password.match(patt);
        if (result != null && result.length > 0) {
            return true;
        } else return false;

    }

    ContainsNumber(password: string) {
        var patt = /[0-9]/g;
        var result = password.match(patt);
        if (result != null && result.length > 0) {
            return true;
        } else return false;
    }

    ContainsSpecialCharacter(password: string) {
        var patt = /[`~!@#$%\^&\*\(\)\-_=\+\{\}\[\]\|\\:;"',\.\/<>\?]/g;
        var result = password.match(patt);
        if (result != null && result.length > 0) {
            return true;
        } else return false;
    }

}