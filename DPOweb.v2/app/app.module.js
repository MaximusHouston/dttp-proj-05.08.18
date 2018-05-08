"use strict";
var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
Object.defineProperty(exports, "__esModule", { value: true });
var core_1 = require("@angular/core");
var platform_browser_1 = require("@angular/platform-browser");
var animations_1 = require("@angular/platform-browser/animations");
//Added when upgraded Kendo UI & Angular 5 (3/26/2018)
var http_1 = require("@angular/common/http");
var http_2 = require("@angular/http");
//import { JsonpModule } from '@angular/http';
var forms_1 = require("@angular/forms");
var ngx_progressbar_1 = require("ngx-progressbar");
var ng_block_ui_1 = require("ng-block-ui");
var common_1 = require("@angular/common");
var app_component_1 = require("./app.component");
var app_routes_1 = require("./app.routes");
//Kendo UI - angular 2 components
var kendo_angular_buttons_1 = require("@progress/kendo-angular-buttons");
var kendo_angular_grid_1 = require("@progress/kendo-angular-grid");
//import { process } from '@progress/kendo-data-query';
//import { orderBy} from '@progress/kendo-data-query';
//import { filterBy } from '@progress/kendo-data-query';
//import { groupBy } from '@progress/kendo-data-query';
//import { aggregateBy } from '@progress/kendo-data-query';
var kendo_angular_dropdowns_1 = require("@progress/kendo-angular-dropdowns");
var kendo_angular_popup_1 = require("@progress/kendo-angular-popup");
var kendo_angular_inputs_1 = require("@progress/kendo-angular-inputs");
var kendo_angular_dateinputs_1 = require("@progress/kendo-angular-dateinputs");
var kendo_angular_layout_1 = require("@progress/kendo-angular-layout");
var kendo_angular_dialog_1 = require("@progress/kendo-angular-dialog");
var kendo_angular_upload_1 = require("@progress/kendo-angular-upload");
var modal_module_1 = require("./shared/modal/modal.module");
var BaseErrorHandler_component_1 = require("./shared/common/BaseErrorHandler.component");
var toastr_service_1 = require("./shared/services/toastr.service");
var account_service_1 = require("./account/services/account.service");
var user_service_1 = require("./shared/services/user.service");
var user_resolver_service_1 = require("./account/services/user-resolver.service");
var user_resolver_service_2 = require("./account/services/user-resolver.service");
var address_service_1 = require("./address/services/address.service");
var quote_service_1 = require("./quote/services/quote.service");
var quote_resolver_service_1 = require("./quote/services/quote-resolver.service");
var quote_resolver_service_2 = require("./quote/services/quote-resolver.service");
var quote_resolver_service_3 = require("./quote/services/quote-resolver.service");
var commissionRequest_service_1 = require("./commissionRequest/services/commissionRequest.service");
var common_service_1 = require("./shared/services/common.service");
var home_component_1 = require("./home/home.component");
var login_component_1 = require("./account/login.component");
var user_registration_component_1 = require("./account/user-registration.component");
var user_personal_details_component_1 = require("./account/user-personal-details.component");
var user_business_details_component_1 = require("./account/user-business-details.component");
var registration_acknowledgement_component_1 = require("./account/registration-acknowledgement.component");
var projectTabs_component_1 = require("./shared/projectTabs/projectTabs.component");
//Projects
var project_service_1 = require("./projects/services/project.service");
var project_resolver_service_1 = require("./project/services/project-resolver.service");
var project_resolver_service_2 = require("./project/services/project-resolver.service");
var projectEdit_component_1 = require("./project/projectEdit.component");
var address_component_1 = require("./address/address.component");
var address_edit_component_1 = require("./address/address-edit.component");
var projectInternal_component_1 = require("./project/projectInternal.component");
var project_component_1 = require("./project/project.component");
var project_pipeline_notes_update_component_1 = require("./project/project-pipeline-notes-update.component");
var projectQuotes_component_1 = require("./project/projectQuotes.component");
var projects_component_1 = require("./projects/projects.component");
var projectGrid_component_1 = require("./projects/projectGrid.component");
var headerButtons_component_1 = require("./shared/header/headerButtons.component");
var transferProjectPopup_component_1 = require("./projects/transferProjectPopup.component");
var deleteProjectsPopup_component_1 = require("./projects/deleteProjectsPopup.component");
var exportProjectsPopup_component_1 = require("./projects/exportProjectsPopup.component");
//Quote
var quote_edit_component_1 = require("./quote/quote-edit.component");
var quote_component_1 = require("./quote/quote.component");
var active_quote_info_component_1 = require("./quote/active-quote-info.component");
var quote_button_bar_component_1 = require("./quote/quote-button-bar.component");
var quote_details_component_1 = require("./quote/quote-details.component");
var quote_items_list_component_1 = require("./quote/quote-items-list.component");
var option_items_component_1 = require("./quote/option-items.component");
var quote_discount_requests_component_1 = require("./quote/quote-discount-requests.component");
var quote_commission_requests_component_1 = require("./quote/quote-commission-requests.component");
var quote_orders_component_1 = require("./quote/quote-orders.component");
var tag_edit_popup_component_1 = require("./quote/tag-edit-popup.component");
var select_products_dialog_component_1 = require("./quote/select-products-dialog.component");
var import_products_dialog_component_1 = require("./quote/import-products-dialog.component");
var add_import_products_dialog_component_1 = require("./quote/add-import-products-dialog.component");
//Commission Request
var calculate_commission_dialog_component_1 = require("./commissionRequest/calculate-commission-dialog.component");
//Products
var products_component_1 = require("./products/products.component");
var productList_component_1 = require("./products/productList.component");
var product_details_listView_component_1 = require("./products/product-details-listView.component");
var product_details_tableView_component_1 = require("./products/product-details-tableView.component");
var product_details_gridView_component_1 = require("./products/product-details-gridView.component");
var product_image_component_1 = require("./products/product-image.component");
var product_spec_bars_component_1 = require("./products/product-spec-bars.component");
var product_quantity_input_component_1 = require("./products/product-quantity-input.component");
var product_quantity_add_component_1 = require("./products/product-quantity-add.component");
var product_details_component_1 = require("./products/productDetails/product-details.component");
var product_overview_component_1 = require("./products/productDetails/product-overview.component");
var product_overview_info_component_1 = require("./products/productDetails/product-overview-info.component");
var related_documents_accessories_component_1 = require("./products/productDetails/related-documents-accessories.component");
var product_accessories_component_1 = require("./products/productDetails/product-accessories.component");
var technical_specifications_component_1 = require("./products/productDetails/technicalSpecifications/technical-specifications.component");
var technical_specifications_accessories_component_1 = require("./products/productDetails/technicalSpecifications/technical-specifications-accessories.component");
var technical_specifications_other_component_1 = require("./products/productDetails/technicalSpecifications/technical-specifications-other.component");
var technical_specifications_systemHP_component_1 = require("./products/productDetails/technicalSpecifications/technical-specifications-systemHP.component");
var basket_component_1 = require("./basket/basket.component");
//Tools - System Configurator
var tools_component_1 = require("./tools/tools.component");
var system_configurator_component_1 = require("./tools/systemConfigurator/system-configurator.component");
var matchup_master_grid_component_1 = require("./tools/systemConfigurator/matchup-master-grid.component");
var matchup_detail_grid_component_1 = require("./tools/systemConfigurator/matchup-detail-grid.component");
var furnaceDDL_component_1 = require("./tools/systemConfigurator/furnaceDDL.component");
// Split System Configurator
var split_system_configurator_component_1 = require("./tools/splitSystemConfigurator/split-system-configurator.component");
var split_matchup_master_grid_component_1 = require("./tools/splitSystemConfigurator/split-matchup-master-grid.component");
var split_matchup_detail_grid_component_1 = require("./tools/splitSystemConfigurator/split-matchup-detail-grid.component");
//Order
var order_component_1 = require("./order/order.component");
var order_form_component_1 = require("./order/order-form.component");
var order_form_quote_items_component_1 = require("./order/order-form-quote-items.component");
var order_service_1 = require("./order/services/order.service");
var order_resolver_service_1 = require("./order/services/order-resolver.service");
var edit_project_location_component_1 = require("./order/edit-project-location.component");
var edit_customer_address_component_1 = require("./order/edit-customer-address.component");
var edit_seller_address_component_1 = require("./order/edit-seller-address.component");
//DiscountRequest
var discountRequest_service_1 = require("./discountRequest/services/discountRequest.service");
var discount_request_component_1 = require("./discountRequest/discount-request.component");
//Submittal Package
var submittal_package_component_1 = require("./submittal-package/submittal-package.component");
var submittal_package_service_1 = require("./submittal-package/services/submittal-package.service");
var upload_files_component_1 = require("./submittal-package/upload-files.component");
var document_and_files_component_1 = require("./submittal-package/document-and-files.component");
//Pipes
var keep_html_pipe_1 = require("./shared/pipes/keep-html.pipe");
var AppModule = /** @class */ (function () {
    function AppModule() {
    }
    AppModule = __decorate([
        core_1.NgModule({
            imports: [
                platform_browser_1.BrowserModule,
                forms_1.FormsModule,
                forms_1.ReactiveFormsModule,
                ngx_progressbar_1.NgProgressModule,
                ng_block_ui_1.BlockUIModule.forRoot(),
                app_routes_1.AppRoutingModule,
                kendo_angular_buttons_1.ButtonsModule,
                animations_1.BrowserAnimationsModule,
                http_1.HttpClientModule,
                http_2.HttpModule,
                http_2.JsonpModule,
                kendo_angular_layout_1.LayoutModule,
                kendo_angular_grid_1.GridModule,
                kendo_angular_dropdowns_1.DropDownsModule,
                kendo_angular_popup_1.PopupModule,
                kendo_angular_inputs_1.InputsModule,
                kendo_angular_dateinputs_1.DateInputsModule,
                kendo_angular_dialog_1.DialogModule,
                kendo_angular_upload_1.UploadModule,
                modal_module_1.ModalModule
            ],
            declarations: [
                app_component_1.AppComponent,
                home_component_1.HomeComponent,
                login_component_1.LoginComponent,
                user_registration_component_1.UserRegistrationComponent,
                user_personal_details_component_1.UserPersonalDetailsComponent,
                user_business_details_component_1.UserBusinessDetailsComponent,
                registration_acknowledgement_component_1.RegistrationAcknowledgementComponent,
                projectTabs_component_1.ProjectTabsComponent,
                projectEdit_component_1.ProjectEditComponent,
                address_component_1.AddressComponent,
                address_edit_component_1.AddressEditComponent,
                projectInternal_component_1.ProjectInternalComponent,
                project_component_1.ProjectComponent,
                project_pipeline_notes_update_component_1.ProjectPipelineNotesUpdateComponent,
                projectQuotes_component_1.ProjectQuotesComponent,
                projects_component_1.ProjectsComponent,
                headerButtons_component_1.HeaderButtonsComponent,
                projectGrid_component_1.ProjectGridComponent,
                transferProjectPopup_component_1.TransferProjectPopupComponent,
                deleteProjectsPopup_component_1.DeleteProjectsPopupComponent,
                exportProjectsPopup_component_1.ExportProjectsPopupComponent,
                quote_edit_component_1.QuoteEditComponent,
                quote_component_1.QuoteComponent,
                active_quote_info_component_1.ActiveQuoteInfoComponent,
                quote_button_bar_component_1.QuoteButtonBarComponent,
                quote_details_component_1.QuoteDetailsComponent,
                quote_items_list_component_1.QuoteItemsListComponent,
                option_items_component_1.OptionItemsComponent,
                quote_discount_requests_component_1.QuoteDiscountRequestsComponent,
                quote_commission_requests_component_1.QuoteCommissionRequestsComponent,
                quote_orders_component_1.QuoteOrdersComponent,
                tag_edit_popup_component_1.TagEditPopupComponent,
                select_products_dialog_component_1.SelectProductsDialogComponent,
                import_products_dialog_component_1.ImportProductsDialogComponent,
                add_import_products_dialog_component_1.AddImportProductsDialogComponent,
                calculate_commission_dialog_component_1.CalculateCommissionDialogComponent,
                products_component_1.ProductsComponent,
                productList_component_1.ProductListComponent,
                product_details_listView_component_1.ProductDetailsListViewComponent,
                product_details_tableView_component_1.ProductDetailsTableViewComponent,
                product_details_gridView_component_1.ProductDetailsGridViewComponent,
                product_image_component_1.ProductImageComponent,
                product_spec_bars_component_1.ProductSpecBarsComponent,
                product_quantity_input_component_1.ProductQuantityInputComponent,
                product_quantity_add_component_1.ProductQuantityAddComponent,
                product_details_component_1.ProductDetailsComponent,
                product_overview_component_1.ProductOverviewComponent,
                product_overview_info_component_1.ProductOverviewInfoComponent,
                related_documents_accessories_component_1.RelatedDocsAndAssrComponent,
                product_accessories_component_1.ProductAccessoriesComponent,
                technical_specifications_component_1.TechnicalSpecificationsComponent,
                technical_specifications_accessories_component_1.TechnicalSpecificationsAccessoriesComponent,
                technical_specifications_other_component_1.TechnicalSpecificationsOtherComponent,
                technical_specifications_systemHP_component_1.TechnicalSpecificationsSystemHPComponent,
                basket_component_1.BasketComponent,
                tools_component_1.ToolsComponent,
                system_configurator_component_1.SystemConfiguratorComponent,
                matchup_master_grid_component_1.MatchupMasterGridComponent,
                matchup_detail_grid_component_1.MatchupDetailGridComponent,
                furnaceDDL_component_1.FurnaceDDLComponent,
                split_system_configurator_component_1.SplitSystemConfiguratorComponent,
                split_matchup_master_grid_component_1.SplitMatchupMasterGridComponent,
                split_matchup_detail_grid_component_1.SplitMatchupDetailGridComponent,
                order_component_1.OrderComponent,
                order_form_component_1.OrderFormComponent,
                order_form_quote_items_component_1.OrderFormQuoteItemsComponent,
                edit_project_location_component_1.EditProjectLocationComponent,
                edit_customer_address_component_1.EditCustomerAddressComponent,
                edit_seller_address_component_1.EditSellerAddressComponent,
                discount_request_component_1.DiscountRequestComponent,
                submittal_package_component_1.SubmittalPackageComponent,
                upload_files_component_1.UploadFilesComponent,
                document_and_files_component_1.DocumentAndFilesComponent,
                BaseErrorHandler_component_1.BaseErrorHandler,
                keep_html_pipe_1.KeepHtmlPipe
            ],
            entryComponents: [tag_edit_popup_component_1.TagEditPopupComponent],
            providers: [{ provide: common_1.LocationStrategy, useClass: common_1.HashLocationStrategy },
                common_service_1.CommonService, toastr_service_1.ToastrService,
                account_service_1.AccountService,
                user_service_1.UserService,
                user_resolver_service_1.UserResolver,
                user_resolver_service_2.CurrentUserResolver,
                address_service_1.AddressService,
                project_service_1.ProjectService,
                project_resolver_service_1.ProjectResolver,
                project_resolver_service_2.ProjectQuotesResolver,
                quote_service_1.QuoteService,
                quote_resolver_service_1.QuoteResolver,
                quote_resolver_service_2.QuoteEditResolver,
                quote_resolver_service_3.QuoteItemsResolver,
                discountRequest_service_1.DiscountRequestService,
                commissionRequest_service_1.CommissionRequestService,
                submittal_package_service_1.SubmittalPackageService,
                order_service_1.OrderService,
                order_resolver_service_1.OrderResolver
            ],
            bootstrap: [app_component_1.AppComponent]
        })
    ], AppModule);
    return AppModule;
}());
exports.AppModule = AppModule;
//# sourceMappingURL=app.module.js.map