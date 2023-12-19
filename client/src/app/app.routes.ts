
import { RouterModule, Routes } from '@angular/router';
import {LoginComponent} from "./pages/login/login.component";
import {CreateAccountComponent} from "./pages/create-account/create-account.component";
import{HomeComponent} from "./pages/home/home.component";
import {ProductsComponent} from "./pages/products/products.component";
import {ProductInfoComponent} from "./pages/product-info/product-info.component";

export const routes: Routes = [
  { path: 'login', component: LoginComponent },
  { path: 'create-account', component: CreateAccountComponent },
  { path: 'home', component: HomeComponent },
  { path: '', component: ProductsComponent },
  { path: 'product-info', component: ProductInfoComponent },

];
