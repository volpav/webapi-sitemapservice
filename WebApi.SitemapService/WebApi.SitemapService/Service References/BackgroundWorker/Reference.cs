﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.18051
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace WebApi.SitemapService.BackgroundWorker {
    using System.Runtime.Serialization;
    using System;
    
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="SitemapNode", Namespace="http://schemas.datacontract.org/2004/07/WebApi.BackgroundWorker")]
    [System.SerializableAttribute()]
    public partial class SitemapNode : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged {
        
        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private WebApi.SitemapService.BackgroundWorker.SitemapNode[] ChildrenField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string TitleField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string UrlField;
        
        [global::System.ComponentModel.BrowsableAttribute(false)]
        public System.Runtime.Serialization.ExtensionDataObject ExtensionData {
            get {
                return this.extensionDataField;
            }
            set {
                this.extensionDataField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public WebApi.SitemapService.BackgroundWorker.SitemapNode[] Children {
            get {
                return this.ChildrenField;
            }
            set {
                if ((object.ReferenceEquals(this.ChildrenField, value) != true)) {
                    this.ChildrenField = value;
                    this.RaisePropertyChanged("Children");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Title {
            get {
                return this.TitleField;
            }
            set {
                if ((object.ReferenceEquals(this.TitleField, value) != true)) {
                    this.TitleField = value;
                    this.RaisePropertyChanged("Title");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Url {
            get {
                return this.UrlField;
            }
            set {
                if ((object.ReferenceEquals(this.UrlField, value) != true)) {
                    this.UrlField = value;
                    this.RaisePropertyChanged("Url");
                }
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="BackgroundWorker.ISitemapService", CallbackContract=typeof(WebApi.SitemapService.BackgroundWorker.ISitemapServiceCallback), SessionMode=System.ServiceModel.SessionMode.Required)]
    public interface ISitemapService {
        
        [System.ServiceModel.OperationContractAttribute(IsOneWay=true, Action="http://tempuri.org/ISitemapService/BeginParseSitemap")]
        void BeginParseSitemap(string url);
        
        [System.ServiceModel.OperationContractAttribute(IsOneWay=true, Action="http://tempuri.org/ISitemapService/BeginParseSitemap")]
        System.Threading.Tasks.Task BeginParseSitemapAsync(string url);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface ISitemapServiceCallback {
        
        [System.ServiceModel.OperationContractAttribute(IsOneWay=true, Action="http://tempuri.org/ISitemapService/OnProgress")]
        void OnProgress(string url, int percentage);
        
        [System.ServiceModel.OperationContractAttribute(IsOneWay=true, Action="http://tempuri.org/ISitemapService/OnCompleted")]
        void OnCompleted(string url, WebApi.SitemapService.BackgroundWorker.SitemapNode sitemap);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface ISitemapServiceChannel : WebApi.SitemapService.BackgroundWorker.ISitemapService, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class SitemapServiceClient : System.ServiceModel.DuplexClientBase<WebApi.SitemapService.BackgroundWorker.ISitemapService>, WebApi.SitemapService.BackgroundWorker.ISitemapService {
        
        public SitemapServiceClient(System.ServiceModel.InstanceContext callbackInstance) : 
                base(callbackInstance) {
        }
        
        public SitemapServiceClient(System.ServiceModel.InstanceContext callbackInstance, string endpointConfigurationName) : 
                base(callbackInstance, endpointConfigurationName) {
        }
        
        public SitemapServiceClient(System.ServiceModel.InstanceContext callbackInstance, string endpointConfigurationName, string remoteAddress) : 
                base(callbackInstance, endpointConfigurationName, remoteAddress) {
        }
        
        public SitemapServiceClient(System.ServiceModel.InstanceContext callbackInstance, string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(callbackInstance, endpointConfigurationName, remoteAddress) {
        }
        
        public SitemapServiceClient(System.ServiceModel.InstanceContext callbackInstance, System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(callbackInstance, binding, remoteAddress) {
        }
        
        public void BeginParseSitemap(string url) {
            base.Channel.BeginParseSitemap(url);
        }
        
        public System.Threading.Tasks.Task BeginParseSitemapAsync(string url) {
            return base.Channel.BeginParseSitemapAsync(url);
        }
    }
}