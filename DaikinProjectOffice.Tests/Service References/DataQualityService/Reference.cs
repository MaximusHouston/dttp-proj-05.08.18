﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DaikinProjectOffice.Tests.DataQualityService {
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(Namespace="http://goodmanmfg.com/GMC/WebService/DataQualityService", ConfigurationName="DataQualityService.IDataQualityService")]
    public interface IDataQualityService {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://goodmanmfg.com/GMC/WebService/DataQualityService/IDataQualityService/Execu" +
            "te", ReplyAction="http://goodmanmfg.com/GMC/WebService/DataQualityService/IDataQualityService/Execu" +
            "teResponse")]
        [System.ServiceModel.ServiceKnownTypeAttribute(typeof(DPO.Domain.DataQualityService.MetaphoneCompareRequest))]
        [System.ServiceModel.ServiceKnownTypeAttribute(typeof(DPO.Domain.DataQualityService.CleanAddressMultipleRequest))]
        [System.ServiceModel.ServiceKnownTypeAttribute(typeof(DPO.Domain.DataQualityService.CleanAddressRequest))]
        [System.ServiceModel.ServiceKnownTypeAttribute(typeof(DPO.Domain.DataQualityService.MetaphoneRequest))]
        [System.ServiceModel.ServiceKnownTypeAttribute(typeof(DPO.Domain.DataQualityService.CleanPhoneRequest))]
        [System.ServiceModel.ServiceKnownTypeAttribute(typeof(DPO.Domain.DataQualityService.SoundexRequest))]
        [System.ServiceModel.ServiceKnownTypeAttribute(typeof(DPO.Domain.DataQualityService.MetaphoneCompareResponse))]
        [System.ServiceModel.ServiceKnownTypeAttribute(typeof(DPO.Domain.DataQualityService.SoundexResponse))]
        [System.ServiceModel.ServiceKnownTypeAttribute(typeof(DPO.Domain.DataQualityService.CleanAddressResponse))]
        [System.ServiceModel.ServiceKnownTypeAttribute(typeof(DPO.Domain.DataQualityService.CleanPhoneResponse))]
        [System.ServiceModel.ServiceKnownTypeAttribute(typeof(DPO.Domain.DataQualityService.CleanAddressMultipleResponse))]
        [System.ServiceModel.ServiceKnownTypeAttribute(typeof(DPO.Domain.DataQualityService.MetaphoneResponse))]
        DPO.Domain.DataQualityService.DataQualityResponse Execute(DPO.Domain.DataQualityService.DataQualityRequest request);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://goodmanmfg.com/GMC/WebService/DataQualityService/IDataQualityService/Execu" +
            "te", ReplyAction="http://goodmanmfg.com/GMC/WebService/DataQualityService/IDataQualityService/Execu" +
            "teResponse")]
        System.Threading.Tasks.Task<DPO.Domain.DataQualityService.DataQualityResponse> ExecuteAsync(DPO.Domain.DataQualityService.DataQualityRequest request);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface IDataQualityServiceChannel : DaikinProjectOffice.Tests.DataQualityService.IDataQualityService, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class DataQualityServiceClient : System.ServiceModel.ClientBase<DaikinProjectOffice.Tests.DataQualityService.IDataQualityService>, DaikinProjectOffice.Tests.DataQualityService.IDataQualityService {
        
        public DataQualityServiceClient() {
        }
        
        public DataQualityServiceClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public DataQualityServiceClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public DataQualityServiceClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public DataQualityServiceClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        public DPO.Domain.DataQualityService.DataQualityResponse Execute(DPO.Domain.DataQualityService.DataQualityRequest request) {
            return base.Channel.Execute(request);
        }
        
        public System.Threading.Tasks.Task<DPO.Domain.DataQualityService.DataQualityResponse> ExecuteAsync(DPO.Domain.DataQualityService.DataQualityRequest request) {
            return base.Channel.ExecuteAsync(request);
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(Namespace="http://goodmanmfg.com/GMC/WebService/DataQualityService/Json", ConfigurationName="DataQualityService.IDataQualityServiceJson")]
    public interface IDataQualityServiceJson {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://goodmanmfg.com/GMC/WebService/DataQualityService/Json/IDataQualityServiceJ" +
            "son/Execute", ReplyAction="http://goodmanmfg.com/GMC/WebService/DataQualityService/Json/IDataQualityServiceJ" +
            "son/ExecuteResponse")]
        [System.ServiceModel.ServiceKnownTypeAttribute(typeof(DPO.Domain.DataQualityService.MetaphoneCompareRequest))]
        [System.ServiceModel.ServiceKnownTypeAttribute(typeof(DPO.Domain.DataQualityService.CleanAddressMultipleRequest))]
        [System.ServiceModel.ServiceKnownTypeAttribute(typeof(DPO.Domain.DataQualityService.CleanAddressRequest))]
        [System.ServiceModel.ServiceKnownTypeAttribute(typeof(DPO.Domain.DataQualityService.MetaphoneRequest))]
        [System.ServiceModel.ServiceKnownTypeAttribute(typeof(DPO.Domain.DataQualityService.CleanPhoneRequest))]
        [System.ServiceModel.ServiceKnownTypeAttribute(typeof(DPO.Domain.DataQualityService.SoundexRequest))]
        [System.ServiceModel.ServiceKnownTypeAttribute(typeof(DPO.Domain.DataQualityService.MetaphoneCompareResponse))]
        [System.ServiceModel.ServiceKnownTypeAttribute(typeof(DPO.Domain.DataQualityService.SoundexResponse))]
        [System.ServiceModel.ServiceKnownTypeAttribute(typeof(DPO.Domain.DataQualityService.CleanAddressResponse))]
        [System.ServiceModel.ServiceKnownTypeAttribute(typeof(DPO.Domain.DataQualityService.CleanPhoneResponse))]
        [System.ServiceModel.ServiceKnownTypeAttribute(typeof(DPO.Domain.DataQualityService.CleanAddressMultipleResponse))]
        [System.ServiceModel.ServiceKnownTypeAttribute(typeof(DPO.Domain.DataQualityService.MetaphoneResponse))]
        DPO.Domain.DataQualityService.DataQualityResponse Execute(DPO.Domain.DataQualityService.DataQualityRequest request);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://goodmanmfg.com/GMC/WebService/DataQualityService/Json/IDataQualityServiceJ" +
            "son/Execute", ReplyAction="http://goodmanmfg.com/GMC/WebService/DataQualityService/Json/IDataQualityServiceJ" +
            "son/ExecuteResponse")]
        System.Threading.Tasks.Task<DPO.Domain.DataQualityService.DataQualityResponse> ExecuteAsync(DPO.Domain.DataQualityService.DataQualityRequest request);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface IDataQualityServiceJsonChannel : DaikinProjectOffice.Tests.DataQualityService.IDataQualityServiceJson, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class DataQualityServiceJsonClient : System.ServiceModel.ClientBase<DaikinProjectOffice.Tests.DataQualityService.IDataQualityServiceJson>, DaikinProjectOffice.Tests.DataQualityService.IDataQualityServiceJson {
        
        public DataQualityServiceJsonClient() {
        }
        
        public DataQualityServiceJsonClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public DataQualityServiceJsonClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public DataQualityServiceJsonClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public DataQualityServiceJsonClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        public DPO.Domain.DataQualityService.DataQualityResponse Execute(DPO.Domain.DataQualityService.DataQualityRequest request) {
            return base.Channel.Execute(request);
        }
        
        public System.Threading.Tasks.Task<DPO.Domain.DataQualityService.DataQualityResponse> ExecuteAsync(DPO.Domain.DataQualityService.DataQualityRequest request) {
            return base.Channel.ExecuteAsync(request);
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(Namespace="http://goodmanmfg.com/GMC/WebService/DataQualityService/Rest", ConfigurationName="DataQualityService.IDataQualityServiceRest")]
    public interface IDataQualityServiceRest {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://goodmanmfg.com/GMC/WebService/DataQualityService/Rest/IDataQualityServiceR" +
            "est/Execute", ReplyAction="http://goodmanmfg.com/GMC/WebService/DataQualityService/Rest/IDataQualityServiceR" +
            "est/ExecuteResponse")]
        [System.ServiceModel.ServiceKnownTypeAttribute(typeof(DPO.Domain.DataQualityService.MetaphoneCompareRequest))]
        [System.ServiceModel.ServiceKnownTypeAttribute(typeof(DPO.Domain.DataQualityService.CleanAddressMultipleRequest))]
        [System.ServiceModel.ServiceKnownTypeAttribute(typeof(DPO.Domain.DataQualityService.CleanAddressRequest))]
        [System.ServiceModel.ServiceKnownTypeAttribute(typeof(DPO.Domain.DataQualityService.MetaphoneRequest))]
        [System.ServiceModel.ServiceKnownTypeAttribute(typeof(DPO.Domain.DataQualityService.CleanPhoneRequest))]
        [System.ServiceModel.ServiceKnownTypeAttribute(typeof(DPO.Domain.DataQualityService.SoundexRequest))]
        [System.ServiceModel.ServiceKnownTypeAttribute(typeof(DPO.Domain.DataQualityService.MetaphoneCompareResponse))]
        [System.ServiceModel.ServiceKnownTypeAttribute(typeof(DPO.Domain.DataQualityService.SoundexResponse))]
        [System.ServiceModel.ServiceKnownTypeAttribute(typeof(DPO.Domain.DataQualityService.CleanAddressResponse))]
        [System.ServiceModel.ServiceKnownTypeAttribute(typeof(DPO.Domain.DataQualityService.CleanPhoneResponse))]
        [System.ServiceModel.ServiceKnownTypeAttribute(typeof(DPO.Domain.DataQualityService.CleanAddressMultipleResponse))]
        [System.ServiceModel.ServiceKnownTypeAttribute(typeof(DPO.Domain.DataQualityService.MetaphoneResponse))]
        DPO.Domain.DataQualityService.DataQualityResponse Execute(DPO.Domain.DataQualityService.DataQualityRequest request);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://goodmanmfg.com/GMC/WebService/DataQualityService/Rest/IDataQualityServiceR" +
            "est/Execute", ReplyAction="http://goodmanmfg.com/GMC/WebService/DataQualityService/Rest/IDataQualityServiceR" +
            "est/ExecuteResponse")]
        System.Threading.Tasks.Task<DPO.Domain.DataQualityService.DataQualityResponse> ExecuteAsync(DPO.Domain.DataQualityService.DataQualityRequest request);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface IDataQualityServiceRestChannel : DaikinProjectOffice.Tests.DataQualityService.IDataQualityServiceRest, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class DataQualityServiceRestClient : System.ServiceModel.ClientBase<DaikinProjectOffice.Tests.DataQualityService.IDataQualityServiceRest>, DaikinProjectOffice.Tests.DataQualityService.IDataQualityServiceRest {
        
        public DataQualityServiceRestClient() {
        }
        
        public DataQualityServiceRestClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public DataQualityServiceRestClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public DataQualityServiceRestClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public DataQualityServiceRestClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        public DPO.Domain.DataQualityService.DataQualityResponse Execute(DPO.Domain.DataQualityService.DataQualityRequest request) {
            return base.Channel.Execute(request);
        }
        
        public System.Threading.Tasks.Task<DPO.Domain.DataQualityService.DataQualityResponse> ExecuteAsync(DPO.Domain.DataQualityService.DataQualityRequest request) {
            return base.Channel.ExecuteAsync(request);
        }
    }
}
