import { Component } from '@angular/core';
import { SignalR, IConnectionOptions } from 'ng2-signalr';
import 'expose-loader?jQuery!jquery';
import '../../scripts/jquery.signalR.js';

// Service
import { DirectorySearchService } from './directorySearch.service';
@Component({
    selector: 'directory-search',
    templateUrl: './directorySearch.component.html'
})
export class DirectorySearchComponent {
    constructor(private _signalR: SignalR,
        private _directorySearchService: DirectorySearchService) { }

    isFTPConnected: boolean = false;
    errorMessage: any = null;
    messages: Array<any> = new Array();
    ngOnInit() {
        this.subscribeToSignalRHub();
    }

    subscribeToSignalRHub() {
        this._signalR.connect().then((c) => {
            let onMessageSent = c.listenFor('BroadcastMessage');
            onMessageSent.subscribe(message => {
                this.messages.push(message);
            });
        });
    }

    ftpConnect(ip: string, user: string, password: string) {
        this._directorySearchService.getFtpDirectoryStructure(ip, user, password)
            .subscribe(response => {
                this.isFTPConnected = true;
            });
    }
}
