/*
SELECT Users.Email,Brands.*,ProductFamilies.*
FROM  Users 
INNER JOIN   Permissions ON Users.UserId = Permissions.ObjectId 
LEFT OUTER JOIN  Brands ON Permissions.ReferenceId = Brands.BrandId AND Permissions.PermissionTypeId = 20 
LEFT OUTER JOIN  ProductFamilies ON Permissions.ReferenceId = ProductFamilies.ProductFamilyId AND Permissions.PermissionTypeId = 30
WHERE  Permissions.PermissionTypeId in (20,30) and (Users.UserId = 202955605193785344)
*/