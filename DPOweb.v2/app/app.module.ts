import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';

//Added when upgraded Kendo UI & Angular 5 (3/26/2018)
import { HttpClientModule } from '@angular/common/http';
import { HttpModule, JsonpModule } from '@angular/http';
//import { JsonpModule } from '@angular/http';

import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { NgProgressModule } from 'ngx-progressbar'; 
import { BlockUIModule } from 'ng-block-ui';

import { LocationStrategy, HashLocationStrategy } from '@angular/common';
import { AppComponent } from './app.component';
import { AppRoutingModule } from './app.routes';

//Kendo UI - angular 2 components
import { ButtonsModule } from '@progress/kendo-angular-buttons';
import { GridModule } from '@progress/kendo-angular-grid';
//import { process } from '@progress/kendo-data-query';
//import { orderBy} from '@progress/kendo-data-query';
//import { filterBy } from '@progress/kendo-data-query';
//import { groupBy } from '@progress/kendo-data-query';
//import { aggregateBy } from '@progress/kendo-data-query';
import { DropDownsModule } from '@progress/kendo-angular-dropdowns';
import { AutoCompleteComponent } from '@progress/kendo-angular-dropdowns';
import { PopupModule } from '@progress/kendo-angular-popup';
import { InputsModule } from '@progress/kendo-angular-inputs';
import { DateInputsModule } from '@progress/kendo-angular-dateinputs';
import { LayoutModule } from '@progress/kendo-angular-layout';
import { DialogModule } from '@progress/kendo-angular-dialog';
import { UploadModule } from '@progress/kendo-angular-upload'; 
import { ModalModule } from './shared/modal/modal.module'; 

import { BaseErrorHandler } from './shared/common/BaseErrorHandler.component';
import { ToastrService } from './shared/services/toastr.service';
import { AccountService } from './account/services/account.service';
import { UserService } from './shared/services/user.service';
import { UserResolver } from './account/services/user-resolver.service';
import { CurrentUserResolver } from './account/services/user-resolver.service';
import { AddressService } from './address/services/address.service';

import { QuoteService } from './quote/services/quote.service';
import { QuoteResolver } from './quote/services/quote-resolver.service';
import { QuoteEditResolver } from './quote/services/quote-resolver.service';
import { QuoteItemsResolver } from './quote/services/quote-resolver.service';
import { CommissionRequestService } from './commissionRequest/services/commissionRequest.service';
import { CommonService } from './shared/services/common.service';

import { HomeComponent } from './home/home.component';
import { LoginComponent } from './account/login.component';
import { UserRegistrationComponent } from './account/user-registration.component';
import { UserPersonalDetailsComponent } from './account/user-personal-details.component';
import { UserBusinessDetailsComponent } from './account/user-business-details.component';
import { RegistrationAcknowledgementComponent } from './account/registration-acknowledgement.component';
import { ProjectTabsComponent } from './shared/projectTabs/projectTabs.component';

//Projects
import { ProjectService } from './projects/services/project.service';
import { ProjectResolver } from './project/services/project-resolver.service';
import { ProjectQuotesResolver } from './project/services/project-resolver.service';
import { ProjectEditComponent } from './project/projectEdit.component';
import { AddressComponent } from './address/address.component';
import { AddressEditComponent } from './address/address-edit.component';
import { ProjectInternalComponent } from './project/projectInternal.component';

import { ProjectComponent } from './project/project.component';
import { ProjectPipelineNotesUpdateComponent } from './project/project-pipeline-notes-update.component';
import { ProjectQuotesComponent } from './project/projectQuotes.component';
import { ProjectsComponent } from './projects/projects.component';
import { ProjectGridComponent } from './projects/projectGrid.component';
import { HeaderButtonsComponent } from './shared/header/headerButtons.component';
import { TransferProjectPopupComponent } from './projects/transferProjectPopup.component';
import { DeleteProjectsPopupComponent } from './projects/deleteProjectsPopup.component';
import { ExportProjectsPopupComponent } from './projects/exportProjectsPopup.component';

//Quote
import { QuoteEditComponent } from './quote/quote-edit.component';
import { QuoteComponent } from './quote/quote.component';
import { ActiveQuoteInfoComponent } from './quote/active-quote-info.component';
import { QuoteButtonBarComponent } from './quote/quote-button-bar.component';
import { QuoteDetailsComponent } from './quote/quote-details.component';
import { QuoteItemsListComponent } from './quote/quote-items-list.component';
import { OptionItemsComponent } from './quote/option-items.component';
import { QuoteDiscountRequestsComponent } from './quote/quote-discount-requests.component';
import { QuoteCommissionRequestsComponent } from './quote/quote-commission-requests.component';
import { QuoteOrdersComponent } from './quote/quote-orders.component';
import { TagEditPopupComponent } from './quote/tag-edit-popup.component';
import { SelectProductsDialogComponent } from './quote/select-products-dialog.component';
import { ImportProductsDialogComponent } from './quote/import-products-dialog.component';
import { AddImportProductsDialogComponent } from './quote/add-import-products-dialog.component';

//Commission Request
import { CalculateCommissionDialogComponent } from './commissionRequest/calculate-commission-dialog.component';

//Products
import { ProductsComponent } from './products/products.component';
import { ProductListComponent } from './products/productList.component';
import { ProductDetailsListViewComponent } from './products/product-details-listView.component';
import { ProductDetailsTableViewComponent } from './products/product-details-tableView.component';
import { ProductDetailsGridViewComponent } from './products/product-details-gridView.component';
import { ProductImageComponent } from './products/product-image.component';
import { ProductSpecBarsComponent } from './products/product-spec-bars.component';
import { ProductQuantityInputComponent } from './products/product-quantity-input.component';
import { ProductQuantityAddComponent } from './products/product-quantity-add.component';
import { ProductDetailsComponent } from './products/productDetails/product-details.component';
import { ProductOverviewComponent } from './products/productDetails/product-overview.component';
import { ProductOverviewInfoComponent } from './products/productDetails/product-overview-info.component';
import { RelatedDocsAndAssrComponent } from './products/productDetails/related-documents-accessories.component';
import { ProductAccessoriesComponent } from './products/productDetails/product-accessories.component';
import { TechnicalSpecificationsComponent } from './products/productDetails/technicalSpecifications/technical-specifications.component';
import { TechnicalSpecificationsAccessoriesComponent } from './products/productDetails/technicalSpecifications/technical-specifications-accessories.component';
import { TechnicalSpecificationsOtherComponent } from './products/productDetails/technicalSpecifications/technical-specifications-other.component';
import { TechnicalSpecificationsSystemHPComponent } from './products/productDetails/technicalSpecifications/technical-specifications-systemHP.component';
import { BasketComponent } from './basket/basket.component';

//Tools - System Configurator
import { ToolsComponent } from './tools/tools.component';
import { SystemConfiguratorComponent } from './tools/systemConfigurator/system-configurator.component';
import { MatchupMasterGridComponent } from './tools/systemConfigurator/matchup-master-grid.component';
import { MatchupDetailGridComponent } from './tools/systemConfigurator/matchup-detail-grid.component';
import { FurnaceDDLComponent } from './tools/systemConfigurator/furnaceDDL.component';

// Split System Configurator
import { SplitSystemConfiguratorComponent } from './tools/splitSystemConfigurator/split-system-configurator.component';
import { SplitMatchupMasterGridComponent } from './tools/splitSystemConfigurator/split-matchup-master-grid.component';
import { SplitMatchupDetailGridComponent } from './tools/splitSystemConfigurator/split-matchup-detail-grid.component';

//Order
import { OrderComponent } from './order/order.component';
import { OrderFormComponent } from './order/order-form.component';
import { OrderFormQuoteItemsComponent } from './order/order-form-quote-items.component';
import { OrderService } from './order/services/order.service';
import { OrderResolver } from './order/services/order-resolver.service';
import { EditProjectLocationComponent } from './order/edit-project-location.component';
import { EditCustomerAddressComponent } from './order/edit-customer-address.component';
import { EditSellerAddressComponent } from './order/edit-seller-address.component';

//DiscountRequest
import { DiscountRequestService } from './discountRequest/services/discountRequest.service';
import { DiscountRequestComponent } from './discountRequest/discount-request.component';

//Submittal Package
import { SubmittalPackageComponent } from './submittal-package/submittal-package.component';
import { SubmittalPackageService } from './submittal-package/services/submittal-package.service';
import { UploadFilesComponent } from './submittal-package/upload-files.component';
import { DocumentAndFilesComponent } from './submittal-package/document-and-files.component';

//Pipes
import { KeepHtmlPipe } from './shared/pipes/keep-html.pipe';

@NgModule({
    imports: [
        BrowserModule,
        FormsModule,
        ReactiveFormsModule, 
        NgProgressModule,
        BlockUIModule.forRoot(),
        
        AppRoutingModule,
        ButtonsModule,
        BrowserAnimationsModule,
        HttpClientModule,
        HttpModule,
        JsonpModule,

        LayoutModule,
        GridModule, 
        DropDownsModule,
        PopupModule,
        InputsModule,
        DateInputsModule,
        DialogModule,
        UploadModule,
        ModalModule
    ],
    declarations: [
        AppComponent,
        HomeComponent,
        LoginComponent,
        UserRegistrationComponent,
        UserPersonalDetailsComponent,
        UserBusinessDetailsComponent,
        RegistrationAcknowledgementComponent,
        ProjectTabsComponent,
        ProjectEditComponent,
        AddressComponent,
        AddressEditComponent,
        ProjectInternalComponent,
        ProjectComponent,
        ProjectPipelineNotesUpdateComponent,
        ProjectQuotesComponent,
        ProjectsComponent,
        HeaderButtonsComponent,
        ProjectGridComponent,
        TransferProjectPopupComponent,
        DeleteProjectsPopupComponent,
        ExportProjectsPopupComponent,
        QuoteEditComponent,
        QuoteComponent,
        ActiveQuoteInfoComponent,
        QuoteButtonBarComponent,
        QuoteDetailsComponent,
        QuoteItemsListComponent,
        OptionItemsComponent,
        QuoteDiscountRequestsComponent,
        QuoteCommissionRequestsComponent,
        QuoteOrdersComponent,
        TagEditPopupComponent,
        SelectProductsDialogComponent,
        ImportProductsDialogComponent,
        AddImportProductsDialogComponent,
        CalculateCommissionDialogComponent,
        ProductsComponent,
        ProductListComponent,
        ProductDetailsListViewComponent,
        ProductDetailsTableViewComponent,
        ProductDetailsGridViewComponent,
        ProductImageComponent,
        ProductSpecBarsComponent,
        ProductQuantityInputComponent,
        ProductQuantityAddComponent,
        ProductDetailsComponent,
        ProductOverviewComponent,
        ProductOverviewInfoComponent,
        RelatedDocsAndAssrComponent,
        ProductAccessoriesComponent,
        TechnicalSpecificationsComponent,
        TechnicalSpecificationsAccessoriesComponent,
        TechnicalSpecificationsOtherComponent,
        TechnicalSpecificationsSystemHPComponent,
        BasketComponent,
        ToolsComponent,
        SystemConfiguratorComponent,
        MatchupMasterGridComponent,
        MatchupDetailGridComponent,
        FurnaceDDLComponent,
        SplitSystemConfiguratorComponent,
        SplitMatchupMasterGridComponent,
        SplitMatchupDetailGridComponent,
        OrderComponent,
        OrderFormComponent,
        OrderFormQuoteItemsComponent,
        EditProjectLocationComponent,
        EditCustomerAddressComponent,
        EditSellerAddressComponent,
        DiscountRequestComponent,
        SubmittalPackageComponent,
        UploadFilesComponent,
        DocumentAndFilesComponent,
        BaseErrorHandler,
        KeepHtmlPipe
    ],
    entryComponents: [TagEditPopupComponent],
    providers: [{ provide: LocationStrategy, useClass: HashLocationStrategy },
        CommonService, ToastrService,
        AccountService,
        UserService,
        UserResolver,
        CurrentUserResolver,
        AddressService,
        ProjectService,
        ProjectResolver,
        ProjectQuotesResolver,
        QuoteService,
        QuoteResolver,
        QuoteEditResolver,
        QuoteItemsResolver,
        DiscountRequestService,
        CommissionRequestService,
        SubmittalPackageService,
        OrderService,
        OrderResolver
    ],
    bootstrap: [AppComponent]
})
export class AppModule { }