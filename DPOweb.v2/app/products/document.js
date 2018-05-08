"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
var Document = /** @class */ (function () {
    function Document(productId, productNumber, documentId, description, type, hasImage, documentUrl, absolutePath, fileName, documentTypeId, rank) {
        this.productId = productId;
        this.productNumber = productNumber;
        this.documentId = documentId;
        this.description = description;
        this.type = type;
        this.hasImage = hasImage;
        this.documentUrl = documentUrl;
        this.absolutePath = absolutePath;
        this.fileName = fileName;
        this.documentTypeId = documentTypeId;
        this.rank = rank;
    }
    return Document;
}());
exports.Document = Document;
//# sourceMappingURL=document.js.map