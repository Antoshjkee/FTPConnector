import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';
import { UniversalModule } from 'angular2-universal';
import { AppComponent } from './components/app/app.component'
import { SignalRModule, SignalRConfiguration } from 'ng2-signalr';
import { DirectorySearchModule } from './components/directorySearch/directorySearch.module';
export function createConfig(): SignalRConfiguration {
    const c = new SignalRConfiguration();
    c.hubName = "FileMonitoringHub";
    c.url = 'http://localhost:5000';
    //c.url = 'http://ftpconnector20170411012042.azurewebsites.net';
    c.logging = true;
    return c;
}

@NgModule({
    bootstrap: [AppComponent],
    declarations: [
        AppComponent
    ],
    imports: [
        UniversalModule, // Must be first import. This automatically imports BrowserModule, HttpModule, and JsonpModule too.
        RouterModule.forRoot([
            { path: '', redirectTo: 'home', pathMatch: 'full' },
            { path: '**', redirectTo: 'home' }
        ]),
        SignalRModule.forRoot(createConfig),
        DirectorySearchModule
    ]
})

export class AppModule {
}
