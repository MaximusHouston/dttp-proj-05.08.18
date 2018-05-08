using DPO.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;

namespace DPO.Services.PIM.Domain
{
    public class PIMParser : PIMXmlParserBase
    {
        private const string DEFAULT_REQUIREMENT_TYPE = "CRT-110959";
        private const string PRODUCT_USER_TYPE_ID = "Item";
        private const string ASSET_TYPE_ROOT = "Asset user-type root";


        private Dictionary<string, string> m_AttributeIDPatterns;
        private Dictionary<string, PIMAttribute> m_Attributes;
        private Dictionary<string, PIMListOfValue> m_ListsOfValues;
        private Dictionary<string, PIMAsset> m_Assets;
        private Dictionary<string, PIMAssetType> m_AssetTypes;
        private Dictionary<string, PIMAssetCrossReferenceType> m_AssetCrossReferenceTypes;

        public PIMParser(string xmlDocLocation)
            : base(xmlDocLocation)
        {
        }

        public PIMParser(XDocument doc)
            : base(doc)
        {
        }

        protected Dictionary<string, string> AttributeIDPatterns
        {
            get
            {
                if (m_AttributeIDPatterns == null)
                {
                    m_AttributeIDPatterns = new Dictionary<string, string>();
                }

                return m_AttributeIDPatterns;
            }
        }

        protected Dictionary<string, PIMAttribute> Attributes
        {
            get
            {
                if (m_Attributes == null)
                {
                    m_Attributes = GetAttributes();
                }

                return m_Attributes;
            }
        }

        protected Dictionary<string, PIMListOfValue> ListsOfValues
        {
            get

            {
                if (m_ListsOfValues == null)
                {
                    m_ListsOfValues = GetListOfValues();
                }

                return m_ListsOfValues;
            }
        }

        protected Dictionary<string, PIMAsset> Assets
        {
            get
            {
                if (m_Assets == null)
                {
                    m_Assets = GetAssets();
                }

                return m_Assets;
            }
        }

        #region Metadata

        public Dictionary<string, PIMAttribute> GetAttributes()
        {
            if (m_Attributes != null)
            {
                return m_Attributes;
            }
            m_Attributes = new Dictionary<string, PIMAttribute>();

            var lovs = this.ListsOfValues;

            var qryAttr = XmlDocument.XPathSelectElements("//AttributeList/Attribute");

            foreach (var attrEl in qryAttr)
            {
                var attribute = ParseAttribute(attrEl, lovs);
                AddAttributeIDPattern(attribute);

                if (attribute != null)
                {
                    m_Attributes.Add(attribute.ID, attribute);
                }
            }

            return m_Attributes;
        }

        /// <summary>
        ///   <ListsOfValues><ListOfValue ...></ListOfValue></ListsOfValues>
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, PIMListOfValue> GetListOfValues()
        {
            if (m_ListsOfValues != null)
            {
                return m_ListsOfValues;
            }

            m_ListsOfValues = new Dictionary<string, PIMListOfValue>();
            var qryLovs = XmlDocument.XPathSelectElements("//ListsOfValues/ListOfValue");

            foreach (var lovEl in qryLovs)
            {
                var lov = ParseListOfValue(lovEl);
                if (lov != null)
                {
                    m_ListsOfValues.Add(lov.ID, lov);
                }
            }

            return m_ListsOfValues;
        }

        private void AddAttributeIDPattern(PIMAttribute attribute)
        {

            if (attribute == null)
            {
                return;
            }

            if (attribute.ListOfValues != null
                && attribute.ListOfValues.UseValueID
                && !String.IsNullOrWhiteSpace(attribute.ListOfValues.IDPattern)
                && !AttributeIDPatterns.ContainsKey(attribute.ID))
            {
                AttributeIDPatterns.Add(attribute.ID, attribute.ListOfValues.IDPattern);
            }
        }

        /// <summary>
        ///     <Attribute ID="CabinetFinish" MultiValued="false" ProductMode="Normal" FullTextIndexed="false" ExternallyMaintained="false" Derived="false" HierarchicalFiltering="false" ClassificationHierarchicalFiltering="false" Selected="true" Referenced="true">
        ///         <Name>Cabinet Finish</Name>
        ///             <ListOfValueLink ListOfValueID = "CabinetFinish" />
        ///             <MetaData>
        ///                 <Value AttributeID= "AttributeHelpText"> Definition: The cabinet finish type.</Value>
        ///                 <Value AttributeID = "DisplaySequence"> 8 </Value>
        ///             </MetaData>
        ///             <AttributeGroupLink AttributeGroupID= "EngineeringGeneralSpecifications" />
        ///             <UserTypeLink UserTypeID= "MajorRevision" />
        ///             <UserTypeLink UserTypeID= "Item" />
        ///     </Attribute>
        /// </summary>
        /// <param name="attrEl"></param>
        /// <returns></returns>
        private PIMAttribute ParseAttribute(XElement attrEl, Dictionary<string, PIMListOfValue> lovs)
        {
            if (attrEl == null)
            {
                return null;
            }

            var id = GetAttributeValue(attrEl, "ID");
            if (String.IsNullOrWhiteSpace(id))
            {
                return null;
            }


            var attribute = new PIMAttribute()
            {
                ID = id
            };
            attribute.Name = GetNodeText(attrEl.XPathSelectElement("Name"));

            // Match up the LOV
            var lovAttr = attrEl.XPathSelectElement("ListOfValueLink");
            if (lovAttr != null)
            {
                var lovID = GetAttributeValue(lovAttr, "ListOfValueID");
                if (lovID != null && lovs.ContainsKey(lovID))
                {
                    attribute.ListOfValues = lovs[lovID];
                }
            }

            return attribute;
        }

        /// <summary>
        ///   <ListOfValue ID="SIDE" ParentID="List Of Values group root" AllowUserValueAddition="false" UseValueID="true" IDPattern="S-[id]">
        ///    <Name>Side</Name>
        ///    <Validation BaseType = "text" MinValue="" MaxValue="" MaxLength="100" InputMask=""/>
        ///    <Value ID = "S-111272" > Top </ Value >
        ///    <Value ID="S-111270">Left</Value>
        ///    <Value ID = "S-111271" > Right </ Value >
        ///    <Value ID="S-111273">Bottom</Value>
        ///   </ListOfValue>
        /// </summary>
        /// <param name="lovEl"></param>
        /// <returns></returns>
        private PIMListOfValue ParseListOfValue(XElement lovEl)
        {
            if (lovEl == null)
            {
                return null;
            }

            var id = GetAttributeValue(lovEl, "ID");
            if (String.IsNullOrWhiteSpace(id))
            {
                return null;
            }

            var useIDsText = GetAttributeValue(lovEl, "UseValueID");
            bool useIDs = false;
            if (!String.IsNullOrWhiteSpace(useIDsText))
            {
                if (!Boolean.TryParse(useIDsText, out useIDs))
                {
                    useIDs = false;
                }
            }

            string idPattern = GetAttributeValue(lovEl, "IDPattern");


            var lov = new PIMListOfValue()
            {
                ID = id,
                UseValueID = useIDs,
                IDPattern = idPattern
            };

            var lovValEls = lovEl.XPathSelectElements("Value");

            foreach (var lovValEl in lovValEls)
            {
                var lovValId = CleanLOVIDWithPattern(idPattern, GetAttributeValue(lovValEl, "ID"));
                var lovVal = GetNodeText(lovValEl);

                // If no ID then use the value for the ID
                if (String.IsNullOrWhiteSpace(lovValId))
                {
                    lovValId = lovVal;
                }

                lov.Values.Add(new KeyValuePair<string, string>(lovValId, lovVal));
            }

            return lov;
        }

        #endregion Metadata

        #region Helper Functions

        private string GetIDPattern(string attributeId)
        {
            string idPattern;

            if (String.IsNullOrWhiteSpace(attributeId))
            {
                return null;
            }

            if (!AttributeIDPatterns.TryGetValue(attributeId, out idPattern))
            {
                return null;
            }

            return idPattern;
        }

        #endregion Helper Functions

        #region Products

        public List<PIMProduct> GetProducts()
        {
            var qry = from lvl in XmlDocument.Descendants("Products")
                      select lvl;

            var products = new List<PIMProduct>();

            m_Attributes = GetAttributes();
            m_Assets = GetAssets();
            m_AssetTypes = GetAssetTypes();

            foreach (var el in qry)
            {
                foreach (var productEl in el.XPathSelectElements("Product"))
                {
                    RecursiveProducts(productEl, products,
                        new Dictionary<string, IPIMProductSpecification>(),
                        new List<PIMProductReference>(),
                        new List<PIMAssetReference>());
                }
            }

            return products;
        }

        private void RecursiveProducts(XElement productEl, List<PIMProduct> products,
           Dictionary<string, IPIMProductSpecification> specs,
           List<PIMProductReference> prodRefs, List<PIMAssetReference> assetRefs)
        {
            var elType = GetAttributeValue(productEl, "UserTypeID");

            MergeSpecifications(productEl, specs);
            MergeMultivalueSpecifications(productEl, specs);
            MergeProductReferences(productEl, prodRefs);
            MergeAssetCrossReferences(productEl, assetRefs);

            // Go through children
            foreach (var el in productEl.XPathSelectElements("Product"))
            {
                // WARNING:  Shallow copy only
                var newSpecs = specs.ToDictionary(entry => entry.Key,
                                           entry => entry.Value);

                var newProdRefs = prodRefs.ToList();

                var newAssetRefs = assetRefs.ToList();

                RecursiveProducts(el, products, newSpecs, newProdRefs, newAssetRefs);
            }

            // Only add the final products
            if (!String.IsNullOrWhiteSpace(elType)
                && elType == PRODUCT_USER_TYPE_ID)
            {
                var id = GetAttributeValue(productEl, "ID");

                // Not a valid product, do not add
                if (id == null)
                {
                    return;
                }

                PIMProduct p = new PIMProduct();
                p.ID = id;

                // Setup Asset Ref Product ID
                p.AssetReferences = new List<PIMAssetReference>();
                foreach (var ar in assetRefs)
                {
                    var myRef = ar.ShallowCopy();
                    myRef.ProductId = id;

                    p.AssetReferences.Add(myRef);
                }

                // Setup Prod Ref Product ID
                p.ProductReferences = new List<PIMProductReference>();
                foreach (var pr in prodRefs)
                {
                    var myProdRef = pr.ShallowCopy();
                    myProdRef.ParentProductNumber = id;

                    p.ProductReferences.Add(myProdRef);
                }

                p.Specifications = specs;

                if (specs.ContainsKey("ReleaseStatus"))
                {
                    var relStat = specs["ReleaseStatus"].Data;
                    p.ReleaseStatus = relStat != null ? Convert.ToInt32(relStat) : 1;
                }

                products.Add(p);
            }
        }

        private void MergeMultivalueSpecifications(XElement el, Dictionary<string, IPIMProductSpecification> specs)
        {
            var multiValueEls = el.XPathSelectElements("Values/MultiValue");

            if (multiValueEls != null)
            {
                foreach (var multiValueEl in multiValueEls)
                {
                    if (multiValueEl == null)
                    {
                        continue;
                    }

                    var attrId = GetAttributeValue(multiValueEl, "AttributeID");
                    if (attrId == null)
                    {
                        continue;
                    }


                    PIMProductMultiValueSpecification multiValueSpec = new PIMProductMultiValueSpecification()
                    {
                        AttributeID = attrId,
                        Value = new List<string>()
                    };

                    var valueEls = multiValueEl.XPathSelectElements("Value");
                    foreach (var valueEl in valueEls)
                    {
                        var specVal = ParseMultiValueSpecification(attrId, valueEl);

                        if (!String.IsNullOrWhiteSpace(specVal))
                        {
                            multiValueSpec.Value.Add(specVal);
                        }
                    }

                    if (multiValueSpec != null)
                    {
                        if (specs.ContainsKey(multiValueSpec.AttributeID))
                        {
                            specs[multiValueSpec.AttributeID] = multiValueSpec;
                        }
                        else
                        {
                            specs.Add(multiValueSpec.AttributeID, multiValueSpec);
                        }
                    }
                }
            }
        }

        private void MergeAssetCrossReferences(XElement el, List<PIMAssetReference> refs)
        {
            var linkedID = GetAttributeValue(el, "ID");
            if (String.IsNullOrWhiteSpace(linkedID))
            {
                return;
            }

            var assetRefEls = el.XPathSelectElements("AssetCrossReference");
            foreach (var assetRefEl in assetRefEls)
            {
                var assetRef = ParseAssetReference(linkedID, assetRefEl);

                if (assetRef != null)
                {
                    // Find if ref already exists, then replace it with the closet to the product itself from hierarchy
                    var existingRef = refs.Where(w => String.Compare(w.AssetId, assetRef.AssetId, true) == 0).FirstOrDefault();

                    if (existingRef != null)
                    {
                        refs.Remove(existingRef);
                    }

                    refs.Add(assetRef);
                }
            }
        }

        private void MergeSpecifications(XElement el, Dictionary<string, IPIMProductSpecification> specs)
        {
            var valueEls = el.XPathSelectElements("Values/Value");

            if (valueEls != null)
            {
                foreach (var valueEl in valueEls)
                {
                    if (valueEl == null)
                    {
                        continue;
                    }

                    var spec = ParseSpecification(valueEl);
                    if (spec != null)
                    {
                        if (specs.ContainsKey(spec.AttributeID))
                        {
                            specs[spec.AttributeID] = spec;
                        }
                        else
                        {
                            specs.Add(spec.AttributeID, spec);
                        }
                    }
                }
            }
        }

        private void MergeProductReferences(XElement el, List<PIMProductReference> prodRefs)
        {
            var linkedID = GetAttributeValue(el, "ID");
            if (String.IsNullOrWhiteSpace(linkedID))
            {
                return;
            }

            var compRefs = el.XPathSelectElements("ProductCrossReference");
            foreach (var compRef in compRefs)
            {
                var prodRef = ParseProductReferences(linkedID, compRef);

                // TODO:  Do we need to override refs that are overriden at the child level.  See MergeAssetCrossRef.
                if (prodRef != null)
                {
                    prodRefs.Add(prodRef);
                }
            }
        }

        private string ParseMultiValueSpecification(string attributeId, XElement el)
        {
            if (el == null)
            {
                return null;
            }

            // First check if this is an LOV and return that as specification
            // otherwise pull the node value
            var id = GetAttributeValue(el, "ID");
            var val = id != null ? CleanLOVIDByAttribute(attributeId, id) : GetNodeText(el);

            if (String.IsNullOrWhiteSpace(val))
            {
                return null;
            }

            return val;
        }

        private PIMProductSpecification ParseSpecification(XElement el)
        {
            if (el == null)
            {
                return null;
            }

            var attrID = GetAttributeValue(el, "AttributeID");
            if (attrID == null)
            {
                return null;
            }

            var spec = new PIMProductSpecification()
            {
                AttributeID = attrID
            };

            // First check if this is an LOV and return that as specification
            // otherwise pull the node value
            var id = GetAttributeValue(el, "ID");
            spec.Value = id != null ? CleanLOVIDByAttribute(attrID, id) : GetNodeText(el);
            spec.RawValue = id != null ? id : GetNodeText(el);
            spec.Text = GetNodeText(el);

            // If the raw value gets removed the re-add
            spec.Value = !String.IsNullOrWhiteSpace(spec.Value) ? spec.Value : spec.RawValue;

            if (String.IsNullOrWhiteSpace(spec.Value))
            {
                return null;
            }

            return spec;
        }

        /// <summary>
        ///     <ProductCrossReference ProductID="CAP-1" Type="Components">
        ///         <MetaData>
        ///             <Value AttributeID="Component Quantity">1</Value>
        ///             <Value AttributeID="Component Requirement Type" ID="CRT-110959">Optional</Value>
        ///         </MetaData>
        ///     </ProductCrossReference>
        /// </summary>
        /// <param name="linkedID"></param>
        /// <param name="compRefEl"></param>
        /// <returns></returns>
        private PIMProductReference ParseProductReferences(string linkedID, XElement compRefEl)
        {
            var xrefType = GetAttributeValue(compRefEl, "Type");
            var componentProdNumber = GetAttributeValue(compRefEl, "ProductID");

            // Skip where not component product
            if (String.IsNullOrWhiteSpace(xrefType) || String.IsNullOrWhiteSpace(componentProdNumber))
            {
                return null;
            }

            var prodRef = new PIMProductReference()
            {
                ReferenceType = xrefType,
                ParentLinkedId = linkedID,
                ProductNumber = componentProdNumber
            };

            var valueEls = compRefEl.XPathSelectElements("MetaData/Value");
            if (valueEls == null)
            {
                return prodRef;
            }

            foreach (var valueEl in valueEls)
            {
                var attrID = GetAttributeValue(valueEl, "AttributeID");
                if (string.IsNullOrWhiteSpace(attrID))
                {
                    continue;
                }

                // Ugly
                switch (attrID.ToLower())
                {
                    case "component quantity":
                        var compQty = GetNodeText(valueEl);

                        int qty;
                        if (Int32.TryParse(compQty, out qty))
                        {
                            prodRef.Quantity = qty;
                        }
                        else
                        {
                            prodRef.Quantity = 1;
                        }
                        break;
                    case "component requirement type":
                        var compReqTypeId = GetAttributeValue(valueEl, "ID");
                        prodRef.ComponentRequirementTypeID = String.IsNullOrWhiteSpace(compReqTypeId) ? DEFAULT_REQUIREMENT_TYPE : CleanLOVIDByAttribute(attrID, compReqTypeId);
                        break;
                }
            }

            return prodRef;
        }

        /// <summary>
        /// <Product ID="ACNF" UserTypeID="Family" ParentID="AH">
        ///         <Name>ACNF</Name>
        ///         <AssetCrossReference AssetID = "PB-DAHFAM-E" Type="ProductBrochure"/>
        ///         ...
        /// </Product>
        /// </summary>
        /// <param name="linkedID"></param>
        /// <param name="assetRefEl"></param>
        /// <returns></returns>
        private PIMAssetReference ParseAssetReference(string linkedID, XElement assetRefEl)
        {
            var xrefType = GetAttributeValue(assetRefEl, "Type");
            var assetID = GetAttributeValue(assetRefEl, "AssetID");

            if (String.IsNullOrWhiteSpace(xrefType)
                   || String.IsNullOrWhiteSpace(assetID)
                   || String.IsNullOrWhiteSpace(linkedID))
            {
                return null;
            }


            var assetRef = new PIMAssetReference()
            {
                LinkedId = linkedID,
                AssetId = assetID
            };

            PIMAsset asset;
            if (Assets.TryGetValue(assetID, out asset))
            {
                assetRef.Asset = asset;
            }

            PIMAssetCrossReferenceType assetXRefType;
            var assetXrefTypes = GetAssetCrossReferenceTypes();
            if (assetXrefTypes != null && assetXrefTypes.TryGetValue(xrefType, out assetXRefType))
            {
                assetRef.AssetCrossReferenceType = assetXRefType;
                assetRef.AssetType = assetXRefType != null ? assetXRefType.AssetType : null;
            }
            else
            {
                Log.ErrorFormat("Unable to parse asset reference for product {0} asset {1} type {2} because asset type doesn't exist", linkedID, assetID, xrefType);
            }

            return assetRef;
        }

       

        #endregion Products

        #region Asset Cross Reference Types

        /// <summary>
        ///  <AssetCrossReferenceType ID="EngineeringDataManual" Inherited="true" Accumulated="false" Revised="true" Mandatory="false" MultiValued="true" Referenced="true">
        ///     <Name>Engineering Data Manual</Name>
        ///     <AttributeGroupLink AttributeGroupID="SupportingDocumentation" />
        ///     <UserTypeLink UserTypeID="SKU"/>
        ///     <UserTypeLink UserTypeID="Family" />
        ///     <UserTypeLink UserTypeID="Item" />
        ///     <TargetUserTypeLink UserTypeID ="EngineeringDataManual" />
        ///  </AssetCrossReferenceType>
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, PIMAssetCrossReferenceType> GetAssetCrossReferenceTypes()
        {
            if (m_AssetCrossReferenceTypes != null)
            {
                return m_AssetCrossReferenceTypes;
            }

            m_AssetCrossReferenceTypes = new Dictionary<string, PIMAssetCrossReferenceType>();

            var qryAssetXrefTypes = XmlDocument.XPathSelectElements("//CrossReferenceTypes/AssetCrossReferenceType");

            var assetTypes = GetAssetTypes();

            // TODO:  Get rid of hard-coded string
            foreach (var assetXrefType in qryAssetXrefTypes)
            {
                var assetXref = ParseAssetCrossReferenceType(assetXrefType, assetTypes);

                if (assetXref != null)
                {
                    m_AssetCrossReferenceTypes.Add(assetXref.Id, assetXref);
                }
            }

            return m_AssetCrossReferenceTypes;
        }

        /// <summary>
        ///  <AssetCrossReferenceType ID="EngineeringDataManual" Inherited="true" Accumulated="false" Revised="true" Mandatory="false" MultiValued="true" Referenced="true">
        ///     <Name>Engineering Data Manual</Name>
        ///     <AttributeGroupLink AttributeGroupID="SupportingDocumentation" />
        ///     <UserTypeLink UserTypeID="SKU"/>
        ///     <UserTypeLink UserTypeID="Family" />
        ///     <UserTypeLink UserTypeID="Item" />
        ///     <TargetUserTypeLink UserTypeID ="EngineeringDataManual" />
        ///  </AssetCrossReferenceType>
        /// </summary>
        /// <returns></returns>
        private PIMAssetCrossReferenceType ParseAssetCrossReferenceType(XElement assetXrefTypeEl,
            Dictionary<string, PIMAssetType> assetTypes)
        {
            if (assetXrefTypeEl == null)
            {
                return null;
            }

            var id = GetAttributeValue(assetXrefTypeEl, "ID");
            var name = GetNodeText(assetXrefTypeEl.Element("Name"));

            var targetUserTypeLinkEl = assetXrefTypeEl.Element("TargetUserTypeLink");
            if (targetUserTypeLinkEl == null)
            {
                return null;
            }

            var targetUserType = GetAttributeValue(targetUserTypeLinkEl, "UserTypeID");

            if (String.IsNullOrWhiteSpace(id)
                || String.IsNullOrWhiteSpace(targetUserType))
            {
                return null;
            }

            PIMAssetType assetType;
            if (!assetTypes.TryGetValue(targetUserType, out assetType))
            {
                return null;
            }

            var assetXrefType = new PIMAssetCrossReferenceType()
            {
                Id = id,
                Name = name,
                AssetType = assetType
            };

            return assetXrefType;
        }


        #endregion Asset Cross Reference Types

        #region Product References

        public List<PIMProductReference> GetProductReferences()
        {
            var qry = from lvl in XmlDocument.Descendants("Product")
                      select lvl;

            List<PIMProductReference> prodRefs = new List<PIMProductReference>();

            // Loop products
            foreach (var item in qry)
            {

                var prodIDAttr = item.Attribute("ID");
                var prodID = prodIDAttr != null ? prodIDAttr.Value : null;

                // Skip where no parent
                if (String.IsNullOrWhiteSpace(prodID))
                {
                    continue;
                }

                var compRefs = item.XPathSelectElements("ProductCrossReference[@Type='Components']");
                foreach (var compRef in compRefs)
                {
                    var productIDAttr = compRef.Attribute("ProductID");
                    string componentProdNumber = productIDAttr != null ? productIDAttr.Value : null;

                    // Skip where not component product
                    if (String.IsNullOrWhiteSpace(componentProdNumber))
                    {
                        continue;
                    }

                    var prodRef = new PIMProductReference()
                    {
                        ParentProductNumber = prodID,
                        ProductNumber = componentProdNumber
                    };
                    prodRefs.Add(prodRef);

                    var compQtyEl = compRef.XPathSelectElement("//Value[@AttributeID='Component Quantity']");

                    int qty;
                    if (compQtyEl != null && Int32.TryParse(compQtyEl.Value, out qty))
                    {
                        prodRef.Quantity = qty;
                    }
                    else
                    {
                        prodRef.Quantity = 1;
                    }

                    var compReqTypeEl = compRef.XPathSelectElement("//Value[@AttributeID='Component Requirement Type']");
                    if (compReqTypeEl != null)
                    {
                        var compReqTypeIDAttr = compReqTypeEl.Attribute("ID");
                        prodRef.ComponentRequirementTypeID = compReqTypeIDAttr == null ? DEFAULT_REQUIREMENT_TYPE : CleanLOVIDByAttribute(null, compReqTypeIDAttr.Value);
                    }
                }
            }

            return prodRefs;
        }

        #endregion Product References

        #region Asset Types

        /// <summary>
        /// <UserType ID="MechanicalGuide" IDPattern="[id]" Referenced="true">
        ///     <Name>Mechanical Guide</Name>
        ///     <Icon></Icon>
        ///     <DimensionLink DimensionID="Language" />
        ///     <DimensionLink DimensionID="Country" />
        ///     <UserTypeLink UserTypeID= "Asset user-type root" />
        /// </UserType>
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, PIMAssetType> GetAssetTypes()
        {
            if (m_AssetTypes != null)
            {
                return m_AssetTypes;
            }
            m_AssetTypes = new Dictionary<string, PIMAssetType>();

            var qryUserTypes = XmlDocument.XPathSelectElements("//UserTypes/UserType");
            m_AssetTypes = new Dictionary<string, PIMAssetType>();

            // TODO:  Get rid of hard-coded string


            foreach (var userTypeEl in qryUserTypes)
            {
                var userTypeLinkEl = userTypeEl.Element("UserTypeLink");

                if (userTypeLinkEl == null)
                {
                    continue;
                }

                // Only get asset types (they must exist in asset root).  Skip if not
                if (String.Compare(GetAttributeValue(userTypeLinkEl, "UserTypeID"), ASSET_TYPE_ROOT, true) != 0)
                {
                    continue;
                }

                // Skip if no id
                var id = GetAttributeValue(userTypeEl, "ID");
                if (String.IsNullOrWhiteSpace(id))
                {
                    continue;
                }

                // Skip if no name element
                var nameEl = userTypeEl.Element("Name");
                if (nameEl == null)
                {
                    continue;
                }

                var assetType = new PIMAssetType()
                {
                    Id = GetAttributeValue(userTypeEl, "ID"),
                    Name = GetNodeText(nameEl)
                };

                m_AssetTypes.Add(assetType.Id, assetType);
            }

            return m_AssetTypes;
        }

        #endregion Asset Types

        #region Assets

        public Dictionary<string, PIMAsset> GetAssets()
        {
            if (m_Assets != null)
            {
                return m_Assets;
            }
            m_Assets = new Dictionary<string, PIMAsset>();

            Dictionary<string, PIMAssetType> assetTypes = GetAssetTypes();

            var qryAssets = XmlDocument.XPathSelectElements("//Assets/Asset");

            foreach (var assetEl in qryAssets)
            {
                var asset = ParseAsset(assetEl, assetTypes);
                if (asset != null)
                {
                    m_Assets.Add(asset.ID, asset);
                }
            }

            return m_Assets;
        }

        /// <summary>
        ///    <Asset ID="PIL-RXTQ36,48TAVJU" UserTypeID="ProductImage" Selected="false" Referenced="true">
        ///      <Name>PIL-RXTQ36,48TAVJU</Name>
        ///      <ClassificationReference ClassificationID="108629"/>
        ///      <Values>
        ///        <Value AttributeID="asset.format">Text (Plain ASCII text)</Value>
        ///        <Value AttributeID="asset.extension">txt</Value>
        ///        <Value AttributeID="asset.size">0</Value>
        ///        <Value AttributeID="asset.mime-type">text/plain; charset=us-ascii</Value>
        ///        <Value AttributeID="asset.filename">PIL-RXTQ36,48TAVJU.jpg</Value>
        ///        <Value AttributeID="asset.uploaded">2017-04-07 14:43:21</Value>
        ///      </Values>
        ///    </Asset>
        /// </summary>
        /// <returns></returns>
        private PIMAsset ParseAsset(XElement assetEl, Dictionary<string, PIMAssetType> assetTypes)
        {
            if (assetEl == null || assetTypes == null)
            {
                return null;
            }

            var assetID = GetAttributeValue(assetEl, "ID");
            var userType = GetAttributeValue(assetEl, "UserTypeID");

            if (String.IsNullOrWhiteSpace(assetID)
                || String.IsNullOrWhiteSpace(userType))
            {
                return null;
            }

            PIMAssetType assetType;

            if (!assetTypes.TryGetValue(userType, out assetType))
            {
                Log.ErrorFormat("Unable to parse asset {0} because asset type {1} does not exist", assetID, userType);
                return null;
            };

            var asset = new PIMAsset()
            {
                ID = assetID,
                AssetType = assetType
            };

            var nameEl = assetEl.XPathSelectElement("Name");
            if (nameEl != null)
            {
                asset.Name = GetNodeText(nameEl);
            }

            var valueEls = assetEl.XPathSelectElements("Values/Value");

            foreach (var valEl in valueEls)
            {
                var attrID = GetAttributeValue(valEl, "AttributeID");

                if (String.IsNullOrWhiteSpace(attrID))
                {
                    continue;
                }

                switch (attrID.ToLower())
                {
                    case "asset.extension":
                        asset.Extension = GetNodeText(valEl);
                        break;
                    case "asset.filename":
                        asset.FileName = GetNodeText(valEl);
                        break;
                    case "asset.description":
                        asset.Description = GetNodeText(valEl);
                        break;
                    case "asset.size":
                        asset.Size = GetNodeText(valEl);
                        break;
                    case "asset.uploaded":
                        DateTime uploadDate;
                        if (DateTime.TryParse(GetNodeText(valEl), out uploadDate))
                        {
                            asset.UploadDate = uploadDate;
                        }
                        else
                        {
                            asset.UploadDate = DateTime.Now;
                        }
                        break;
                }
            }

            return asset;
        }

        #endregion Assets

        private string CleanLOVIDByAttribute(string attributeId, string id)
        {
            return CleanLOVIDWithPattern(GetIDPattern(attributeId), id);
        }
    }
}
