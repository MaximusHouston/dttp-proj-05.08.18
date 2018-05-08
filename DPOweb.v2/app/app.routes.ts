
import { Routes, RouterModule } from '@angular/router';
import { ModuleWithProviders } from '@angular/core';
import { UserResolver } from './account/services/user-resolver.service';
import { CurrentUserResolver } from './account/services/user-resolver.service';
import { ProjectResolver } from './project/services/project-resolver.service';
import { ProjectQuotesResolver } from './project/services/project-resolver.service';
import { ProjectEditComponent } from './project/projectEdit.component';
import { ProjectComponent } from './project/project.component';
import { ProjectsComponent } from './projects/projects.component';
import { HomeComponent } from './home/home.component';
import { LoginComponent } from './account/login.component';
import { UserRegistrationComponent } from './account/user-registration.component';
import { RegistrationAcknowledgementComponent } from './account/registration-acknowledgement.component';
import { QuoteComponent } from './quote/quote.component';
import { QuoteEditComponent } from './quote/quote-edit.component';
import { QuoteResolver } from './quote/services/quote-resolver.service';
import { QuoteEditResolver } from './quote/services/quote-resolver.service';
import { QuoteItemsResolver } from './quote/services/quote-resolver.service';

import { ProductsComponent } from './products/products.component';
import { ProductListComponent } from './products/productList.component';
import { ProductDetailsComponent } from './products/productDetails/product-details.component';

import { SystemConfiguratorComponent } from './tools/systemConfigurator/system-configurator.component';
import { SplitSystemConfiguratorComponent } from './tools/splitSystemConfigurator/split-system-configurator.component';
import { ToolsComponent } from './tools/tools.component';

import { OrderComponent } from './order/order.component';
import { OrderFormComponent } from './order/order-form.component';
import { OrderResolver } from './order/services/order-resolver.service';

import { DiscountRequestComponent } from './discountRequest/discount-request.component';
import { SubmittalPackageComponent } from './submittal-package/submittal-package.component';

const routes: Routes = [
    {
        path: '',
        redirectTo: '/home',
        pathMatch: 'full'
    },
    {
        path: 'home',
        component: HomeComponent,
        resolve: {
            currentUser: CurrentUserResolver
        }
    },
    {
        path: 'account',
        children: [
            { path: '', redirectTo: 'login', pathMatch: 'full' },
            { path: 'login', component: LoginComponent },
            {
                path: 'userRegistration',
                component: UserRegistrationComponent,
                //data: { pageTitle: 'User Registration' },
                resolve: { user: UserResolver }
            }
        ]
    },
    {
        path: 'registrationAcknowledgement', component: RegistrationAcknowledgementComponent
    },
    //{
    //    path: 'account/login', component: LoginComponent
    //},
    //{
    //    path: 'account/userRegistration', component: UserRegistrationComponent
    //},
    {
        path: 'projectCreate',
        component: ProjectEditComponent,
        resolve: {
            projectModel: ProjectResolver,
            currentUser: CurrentUserResolver
        }
    },
    {
        path: 'projectEdit/:id',
        component: ProjectEditComponent,
        resolve: {
            projectModel: ProjectResolver,
            currentUser: CurrentUserResolver
        }
    },
    {
        path: 'project/:id',
        component: ProjectComponent,
        resolve: {
            projectModel: ProjectResolver,
            projectQuotesModel: ProjectQuotesResolver,
            currentUser: CurrentUserResolver
        }
    },
    {
        path: 'projectQuotes/:id',
        component: ProjectComponent,
        resolve: {
            projectModel: ProjectResolver,
            projectQuotesModel: ProjectQuotesResolver,
            currentUser: CurrentUserResolver
        }
    },
    {
        path: 'projects',
        component: ProjectsComponent,
        resolve: {
            currentUser: CurrentUserResolver
        }
    },
    {
        path: 'quoteCreate/:projectid',
        component: QuoteEditComponent,
        resolve: {
            quoteModel: QuoteEditResolver,
            currentUser: CurrentUserResolver
        }
    },
    {
        path: 'quoteEdit/:projectid/:quoteid',
        component: QuoteEditComponent,
        resolve: {
            quoteModel: QuoteEditResolver,
            currentUser: CurrentUserResolver
        }
    },
    {
        path: 'quote/:id/:recordState',
        component: QuoteComponent,
        resolve: {
            quoteModel: QuoteResolver,
            currentUser: CurrentUserResolver
        }
    },
    {
        path: 'quoteItems/:id/:recordState',
        component: QuoteComponent,
        resolve: {
            quoteModel: QuoteResolver,
            quoteItems: QuoteItemsResolver,
            currentUser: CurrentUserResolver
        }
    },
    {
        path: 'products',
        component: ProductsComponent

        , children: [
            //{ path: 'list', component: ProductListComponent, outlet: 'productList' },
            { path: ':id', component: ProductDetailsComponent, outlet: 'productDetails' }
        ]
    },
    {
        path: 'tools',
        component: ToolsComponent,
        resolve: {
            currentUser: CurrentUserResolver
        }
    },
    {
        path: 'tools/systemConfigurator',
        component: SystemConfiguratorComponent,
        resolve: {
            currentUser: CurrentUserResolver
        }
    },
    {
        path: 'tools/splitSystemConfigurator',
        component: SplitSystemConfiguratorComponent,
        resolve: {
            currentUser: CurrentUserResolver
        }
    }, {
        path: 'discountRequest/:discountRequestId/:projectId/:quoteId',
        component: DiscountRequestComponent,
        resolve: {
            currentUser: CurrentUserResolver
        }
    },
    {
        path: 'order',
        component: OrderComponent
    },
    {
        path: 'orderForm/:projectid/:quoteid/:recordState',
        //path: 'orderForm/:projectid/:quoteid',
        //path: 'orderForm',
        component: OrderFormComponent,
        resolve: {
            orderFormModel: OrderResolver,
            currentUser: CurrentUserResolver
        }
    },
    {
        path: 'submittalPackage/:projectid/:quoteid',
        component: SubmittalPackageComponent,
        resolve: {
            quoteModel: QuoteEditResolver,
            currentUser: CurrentUserResolver
        }
    },
];

export const AppRoutingModule: ModuleWithProviders = RouterModule.forRoot(routes);