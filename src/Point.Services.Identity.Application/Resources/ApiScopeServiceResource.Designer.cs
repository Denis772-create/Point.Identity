﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Point.Services.Identity.Application.Resources {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "17.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class ApiScopeServiceResource {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal ApiScopeServiceResource() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Point.Services.Identity.Application.Resources.ApiScopeServiceResource", typeof(ApiScopeServiceResource).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Api Scope with id {0} doesn&apos;t exist.
        /// </summary>
        internal static string ApiScopeDoesNotExist {
            get {
                return ResourceManager.GetString("ApiScopeDoesNotExist", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Api Scope already exists.
        /// </summary>
        internal static string ApiScopeExistsKey {
            get {
                return ResourceManager.GetString("ApiScopeExistsKey", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Api Scope ({0}) already exists.
        /// </summary>
        internal static string ApiScopeExistsValue {
            get {
                return ResourceManager.GetString("ApiScopeExistsValue", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Api Scope Property with id {0} doesn&apos;t exist.
        /// </summary>
        internal static string ApiScopePropertyDoesNotExist {
            get {
                return ResourceManager.GetString("ApiScopePropertyDoesNotExist", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Api Scope Property already exists.
        /// </summary>
        internal static string ApiScopePropertyExistsKey {
            get {
                return ResourceManager.GetString("ApiScopePropertyExistsKey", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Api Scope Property ({0}) already exists.
        /// </summary>
        internal static string ApiScopePropertyExistsValue {
            get {
                return ResourceManager.GetString("ApiScopePropertyExistsValue", resourceCulture);
            }
        }
    }
}
