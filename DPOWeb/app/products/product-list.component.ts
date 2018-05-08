import { Component, OnInit } from 'angular2/core';
import { IProduct } from './product';

@Component({
    selector: 'Daikin-products',
    templateUrl: 'app/products/product-list.component.html',
    styleUrls: ['app/products/product-list.component.css']
})
export class ProductListComponent implements OnInit {
    pageTitle: string = "Product List";
    imageWidth: number = 100;
    imageHeight: number = 100;
    imageMargin: number = 2;
    showImage: boolean = false;
    listFilter: string = "cart";
    products: IProduct[] = [
        {
            "productId": 2,
            "productName": "Garden Cart",
            "productCode": "GDN-0023",
            "releaseDate": "March 18, 2016",
            "description": "15 gallon capacity rolling",
            "price": 32.99,
            "starRating": 4.2,
            "imageUrl" : "./Images/Image2.jpg"
        },
        {
            "productId": 5,
            "productName": "Hammer",
            "productCode": "TBX-0048",
            "releaseDate": "May 21, 2016",
            "description": "Curve claw steel hammer",
            "price": 8.91,
            "starRating": 4.8,
            "imageUrl": "./Images/Image1.jpg"
        },
        {
            "productId": 6,
            "productName": "Solor light",
            "productCode": "SOL-0020",
            "releaseDate": "Jun 06, 2016",
            "description": "No battery need",
            "price": 3.99,
            "starRating": 4.5,
            "imageUrl": "./Images/Image3.jpg"
        }
    ];

    toggleImage(): void {
        this.showImage = !this.showImage;
    }

    ngOnInit(): void {
        console.log('In OnInit');
    }
}  