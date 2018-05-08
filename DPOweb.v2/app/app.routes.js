"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
var router_1 = require("@angular/router");
var user_resolver_service_1 = require("./account/services/user-resolver.service");
var user_resolver_service_2 = require("./account/services/user-resolver.service");
var project_resolver_service_1 = require("./project/services/project-resolver.service");
var project_resolver_service_2 = require("./project/services/project-resolver.service");
var projectEdit_component_1 = require("./project/projectEdit.component");
var project_component_1 = require("./project/project.component");
var projects_component_1 = require("./projects/projects.component");
var home_component_1 = require("./home/home.component");
var login_component_1 = require("./account/login.component");
var user_registration_component_1 = require("./account/user-registration.component");
var registration_acknowledgement_component_1 = require("./account/registration-acknowledgement.component");
var quote_component_1 = require("./quote/quote.component");
var quote_edit_component_1 = require("./quote/quote-edit.component");
var quote_resolver_service_1 = require("./quote/services/quote-resolver.service");
var quote_resolver_service_2 = require("./quote/services/quote-resolver.service");
var quote_resolver_service_3 = require("./quote/services/quote-resolver.service");
var products_component_1 = require("./products/products.component");
var product_details_component_1 = require("./products/productDetails/product-details.component");
var system_configurator_component_1 = require("./tools/systemConfigurator/system-configurator.component");
var split_system_configurator_component_1 = require("./tools/splitSystemConfigurator/split-system-configurator.component");
var tools_component_1 = require("./tools/tools.component");
var order_component_1 = require("./order/order.component");
var order_form_component_1 = require("./order/order-form.component");
var order_resolver_service_1 = require("./order/services/order-resolver.service");
var discount_request_component_1 = require("./discountRequest/discount-request.component");
var submittal_package_component_1 = require("./submittal-package/submittal-package.component");
var routes = [
    {
        path: '',
        redirectTo: '/home',
        pathMatch: 'full'
    },
    {
        path: 'home',
        component: home_component_1.HomeComponent,
        resolve: {
            currentUser: user_resolver_service_2.CurrentUserResolver
        }
    },
    {
        path: 'account',
        children: [
            { path: '', redirectTo: 'login', pathMatch: 'full' },
            { path: 'login', component: login_component_1.LoginComponent },
            {
                path: 'userRegistration',
                component: user_registration_component_1.UserRegistrationComponent,
                //data: { pageTitle: 'User Registration' },
                resolve: { user: user_resolver_service_1.UserResolver }
            }
        ]
    },
    {
        path: 'registrationAcknowledgement', component: registration_acknowledgement_component_1.RegistrationAcknowledgementComponent
    },
    //{
    //    path: 'account/login', component: LoginComponent
    //},
    //{
    //    path: 'account/userRegistration', component: UserRegistrationComponent
    //},
    {
        path: 'projectCreate',
        component: projectEdit_component_1.ProjectEditComponent,
        resolve: {
            projectModel: project_resolver_service_1.ProjectResolver,
            currentUser: user_resolver_service_2.CurrentUserResolver
        }
    },
    {
        path: 'projectEdit/:id',
        component: projectEdit_component_1.ProjectEditComponent,
        resolve: {
            projectModel: project_resolver_service_1.ProjectResolver,
            currentUser: user_resolver_service_2.CurrentUserResolver
        }
    },
    {
        path: 'project/:id',
        component: project_component_1.ProjectComponent,
        resolve: {
            projectModel: project_resolver_service_1.ProjectResolver,
            projectQuotesModel: project_resolver_service_2.ProjectQuotesResolver,
            currentUser: user_resolver_service_2.CurrentUserResolver
        }
    },
    {
        path: 'projectQuotes/:id',
        component: project_component_1.ProjectComponent,
        resolve: {
            projectModel: project_resolver_service_1.ProjectResolver,
            projectQuotesModel: project_resolver_service_2.ProjectQuotesResolver,
            currentUser: user_resolver_service_2.CurrentUserResolver
        }
    },
    {
        path: 'projects',
        component: projects_component_1.ProjectsComponent,
        resolve: {
            currentUser: user_resolver_service_2.CurrentUserResolver
        }
    },
    {
        path: 'quoteCreate/:projectid',
        component: quote_edit_component_1.QuoteEditComponent,
        resolve: {
            quoteModel: quote_resolver_service_2.QuoteEditResolver,
            currentUser: user_resolver_service_2.CurrentUserResolver
        }
    },
    {
        path: 'quoteEdit/:projectid/:quoteid',
        component: quote_edit_component_1.QuoteEditComponent,
        resolve: {
            quoteModel: quote_resolver_service_2.QuoteEditResolver,
            currentUser: user_resolver_service_2.CurrentUserResolver
        }
    },
    {
        path: 'quote/:id/:recordState',
        component: quote_component_1.QuoteComponent,
        resolve: {
            quoteModel: quote_resolver_service_1.QuoteResolver,
            currentUser: user_resolver_service_2.CurrentUserResolver
        }
    },
    {
        path: 'quoteItems/:id/:recordState',
        component: quote_component_1.QuoteComponent,
        resolve: {
            quoteModel: quote_resolver_service_1.QuoteResolver,
            quoteItems: quote_resolver_service_3.QuoteItemsResolver,
            currentUser: user_resolver_service_2.CurrentUserResolver
        }
    },
    {
        path: 'products',
        component: products_component_1.ProductsComponent,
        children: [
            //{ path: 'list', component: ProductListComponent, outlet: 'productList' },
            { path: ':id', component: product_details_component_1.ProductDetailsComponent, outlet: 'productDetails' }
        ]
    },
    {
        path: 'tools',
        component: tools_component_1.ToolsComponent,
        resolve: {
            currentUser: user_resolver_service_2.CurrentUserResolver
        }
    },
    {
        path: 'tools/systemConfigurator',
        component: system_configurator_component_1.SystemConfiguratorComponent,
        resolve: {
            currentUser: user_resolver_service_2.CurrentUserResolver
        }
    },
    {
        path: 'tools/splitSystemConfigurator',
        component: split_system_configurator_component_1.SplitSystemConfiguratorComponent,
        resolve: {
            currentUser: user_resolver_service_2.CurrentUserResolver
        }
    }, {
        path: 'discountRequest/:discountRequestId/:projectId/:quoteId',
        component: discount_request_component_1.DiscountRequestComponent,
        resolve: {
            currentUser: user_resolver_service_2.CurrentUserResolver
        }
    },
    {
        path: 'order',
        component: order_component_1.OrderComponent
    },
    {
        path: 'orderForm/:projectid/:quoteid/:recordState',
        //path: 'orderForm/:projectid/:quoteid',
        //path: 'orderForm',
        component: order_form_component_1.OrderFormComponent,
        resolve: {
            orderFormModel: order_resolver_service_1.OrderResolver,
            currentUser: user_resolver_service_2.CurrentUserResolver
        }
    },
    {
        path: 'submittalPackage/:projectid/:quoteid',
        component: submittal_package_component_1.SubmittalPackageComponent,
        resolve: {
            quoteModel: quote_resolver_service_2.QuoteEditResolver,
            currentUser: user_resolver_service_2.CurrentUserResolver
        }
    },
];
exports.AppRoutingModule = router_1.RouterModule.forRoot(routes);
//# sourceMappingURL=app.routes.js.map