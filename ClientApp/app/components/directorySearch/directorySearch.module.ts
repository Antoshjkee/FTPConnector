import { NgModule } from '@angular/core';
import { DirectorySearchComponent } from "./directorySearch.component";
import { DirectorySearchService } from './directorySearch.service';
import { CommonModule } from '@angular/common';
@NgModule({
    imports: [CommonModule],
    declarations: [DirectorySearchComponent],
    providers: [DirectorySearchService],
    exports: [DirectorySearchComponent]
})
export class DirectorySearchModule {
}