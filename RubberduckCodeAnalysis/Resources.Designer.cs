﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace RubberduckCodeAnalysis {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "15.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    public class Resources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("RubberduckCodeAnalysis.Resources", typeof(Resources).Assembly);
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
        public static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to COM Management.
        /// </summary>
        public static string AnalyzerCategory {
            get {
                return ResourceManager.GetString("AnalyzerCategory", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to All types derived from SafeComWrappers should not be chained as it leaks unmanaged resources. Use an explicit local variable for each chained member..
        /// </summary>
        public static string ChainedWrapperDescription {
            get {
                return ResourceManager.GetString("ChainedWrapperDescription", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The type &apos;{0}&apos; derives from a SafeComWrapper base. It is called via other SafeComWrapper-derived type &apos;{1}&apos; in the expression &apos;{2}&apos;..
        /// </summary>
        public static string ChainedWrapperMessageFormat {
            get {
                return ResourceManager.GetString("ChainedWrapperMessageFormat", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Chained Wrappers.
        /// </summary>
        public static string ChainedWrapperTitle {
            get {
                return ResourceManager.GetString("ChainedWrapperTitle", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to COM-visible classes must have an explicit ClassInterface attribute and be set to `None`. This is required to avoid versioning problems..
        /// </summary>
        public static string MissingClassInterfaceDescription {
            get {
                return ResourceManager.GetString("MissingClassInterfaceDescription", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to MissingClassInterface.
        /// </summary>
        public static string MissingClassInterfaceId {
            get {
                return ResourceManager.GetString("MissingClassInterfaceId", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to COM-visible class &apos;{0}&apos; does not have an explicit ClassInterface attribute that is also set to &apos;ClassInterfaceType.None&apos;..
        /// </summary>
        public static string MissingClassInterfaceMessageFormat {
            get {
                return ResourceManager.GetString("MissingClassInterfaceMessageFormat", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Missing ClassInterface Attribute.
        /// </summary>
        public static string MissingClassInterfaceTitle {
            get {
                return ResourceManager.GetString("MissingClassInterfaceTitle", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to COM-visible classes must have an explicit ComDefaultInterface attribute referring to a COM-visible interface..
        /// </summary>
        public static string MissingComDefaultInterfaceDescription {
            get {
                return ResourceManager.GetString("MissingComDefaultInterfaceDescription", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to MissingComDefaultInterface.
        /// </summary>
        public static string MissingComDefaultInterfaceId {
            get {
                return ResourceManager.GetString("MissingComDefaultInterfaceId", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to COM-visible class &apos;{0}&apos; must have an explicit ComDefaultInterface attribute using a typeof reference to a COM-visible interface. Do not use string to provide the interface name. .
        /// </summary>
        public static string MissingComDefaultInterfaceMessageFormat {
            get {
                return ResourceManager.GetString("MissingComDefaultInterfaceMessageFormat", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Missing ComDefaultInterface Attribute.
        /// </summary>
        public static string MissingComDefaultInterfaceTitle {
            get {
                return ResourceManager.GetString("MissingComDefaultInterfaceTitle", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Missing xml-doc &apos;example&apos; tag.
        /// </summary>
        public static string MissingExampleTag {
            get {
                return ResourceManager.GetString("MissingExampleTag", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Inspections xml-doc should have at least one &apos;example&apos; tag, ideally two. If only one example is provided, website assumes the code example triggers the inspection. If two examples are provided, the second example is assumed to not trigger the inspection. Any further example is assumed to trigger the inspection..
        /// </summary>
        public static string MissingExampleTagDescription {
            get {
                return ResourceManager.GetString("MissingExampleTagDescription", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to XML documentation of type &apos;{0}&apos; has no &lt;example&gt; tag..
        /// </summary>
        public static string MissingExampleTagMessageFormat {
            get {
                return ResourceManager.GetString("MissingExampleTagMessageFormat", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to COM-visible types must have an explicit Guid attribute. This is required to avoid verisoning problems. Refer to RubberduckGuid constants..
        /// </summary>
        public static string MissingGuidDescription {
            get {
                return ResourceManager.GetString("MissingGuidDescription", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to MissingGuid.
        /// </summary>
        public static string MissingGuidId {
            get {
                return ResourceManager.GetString("MissingGuidId", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to COM-visible type &apos;{0}&apos; does not have an explicit Guid attribute that references a RubberduckGuid constant..
        /// </summary>
        public static string MissingGuidMessageFormat {
            get {
                return ResourceManager.GetString("MissingGuidMessageFormat", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Missing Guid Attribute.
        /// </summary>
        public static string MissingGuidTitle {
            get {
                return ResourceManager.GetString("MissingGuidTitle", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &apos;example&apos; element requires a &apos;hasresult&apos; attribute (bool), which isn&apos;t supplied..
        /// </summary>
        public static string MissingHasResultAttribute {
            get {
                return ResourceManager.GetString("MissingHasResultAttribute", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to This attribute value will be used to clearly identify which examples are for code that triggers an inspection, vs examples for code that doesn&apos;t..
        /// </summary>
        public static string MissingHasResultAttributeDescription {
            get {
                return ResourceManager.GetString("MissingHasResultAttributeDescription", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &apos;example&apos; element has no &apos;hasresult&apos; attribute..
        /// </summary>
        public static string MissingHasResultAttributeMessageFormat {
            get {
                return ResourceManager.GetString("MissingHasResultAttributeMessageFormat", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Missing xml-doc &apos;reference&apos; tag.
        /// </summary>
        public static string MissingInspectionReferenceTag {
            get {
                return ResourceManager.GetString("MissingInspectionReferenceTag", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to XML documentation for inspections with a [RequiredLibraryAttribute] must include a &lt;reference&gt; tag with a &apos;name&apos; attribute with the same value as the [RequiredLibraryAttribute]. For example [RequiredLibrary(&quot;Excel&quot;)] mandates &lt;reference name=&quot;Excel&quot; /&gt;..
        /// </summary>
        public static string MissingInspectionReferenceTagDescription {
            get {
                return ResourceManager.GetString("MissingInspectionReferenceTagDescription", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to XML documentation for type &apos;{0}&apos; is missing a &apos;&lt;reference name=&quot;{1}&quot;&gt;&apos; tag..
        /// </summary>
        public static string MissingInspectionReferenceTagMessageFormat {
            get {
                return ResourceManager.GetString("MissingInspectionReferenceTagMessageFormat", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Missing xml-doc &apos;summary&apos; tag.
        /// </summary>
        public static string MissingInspectionSummaryTag {
            get {
                return ResourceManager.GetString("MissingInspectionSummaryTag", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to All inspections must have a short &lt;summary&gt; xml-doc comment describing what the inspection is looking for, that reads comfortably in IntelliSense..
        /// </summary>
        public static string MissingInspectionSummaryTagDescription {
            get {
                return ResourceManager.GetString("MissingInspectionSummaryTagDescription", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to XML documentation for type &apos;{0}&apos; is missing a &lt;summary&gt; tag..
        /// </summary>
        public static string MissingInspectionSummaryTagMessageFormat {
            get {
                return ResourceManager.GetString("MissingInspectionSummaryTagMessageFormat", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Missing xml-doc &apos;why&apos; tag.
        /// </summary>
        public static string MissingInspectionWhyTag {
            get {
                return ResourceManager.GetString("MissingInspectionWhyTag", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Inspections xml-doc must have a &apos;why&apos; tag that contains a paragraph explaining the reasoning behind the inspection..
        /// </summary>
        public static string MissingInspectionWhyTagDescription {
            get {
                return ResourceManager.GetString("MissingInspectionWhyTagDescription", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to XML documentation for type &apos;{0}&apos; is missing a &lt;why&gt; tag..
        /// </summary>
        public static string MissingInspectionWhyTagMessageFormat {
            get {
                return ResourceManager.GetString("MissingInspectionWhyTagMessageFormat", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to COM-visible interfaces must have an explicit InterfaceType attribute, typically set to Dual or Dispatch for event interfaces. .
        /// </summary>
        public static string MissingInterfaceTypeDescription {
            get {
                return ResourceManager.GetString("MissingInterfaceTypeDescription", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to MissingInterfaceType.
        /// </summary>
        public static string MissingInterfaceTypeId {
            get {
                return ResourceManager.GetString("MissingInterfaceTypeId", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to COM-visible interface &apos;{0}&apos; does not have an explicit InterfaceType attribute with the type of interface set. InterfaceIsDual is the recommended choice, unless it&apos;s an event, in which case, InterfaceIsIDispatch is recommended instead..
        /// </summary>
        public static string MissingInterfaceTypeMessageFormat {
            get {
                return ResourceManager.GetString("MissingInterfaceTypeMessageFormat", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Missing InterfaceType Attribute.
        /// </summary>
        public static string MissingInterfaceTypeTitle {
            get {
                return ResourceManager.GetString("MissingInterfaceTypeTitle", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to XML documentation example is missing a &apos;module&apos; tag..
        /// </summary>
        public static string MissingModuleTag {
            get {
                return ResourceManager.GetString("MissingModuleTag", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to All inspection xml-doc examples should include one or more &apos;module&apos; tag with a &apos;name&apos; attribute..
        /// </summary>
        public static string MissingModuleTagDescription {
            get {
                return ResourceManager.GetString("MissingModuleTagDescription", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &apos;example&apos; element has no &apos;module&apos; child..
        /// </summary>
        public static string MissingModuleTagMessageFormat {
            get {
                return ResourceManager.GetString("MissingModuleTagMessageFormat", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Missing &apos;name&apos; attribute..
        /// </summary>
        public static string MissingNameAttribute {
            get {
                return ResourceManager.GetString("MissingNameAttribute", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to This attribute value will be used to clearly identify each module in a code example..
        /// </summary>
        public static string MissingNameAttributeDescription {
            get {
                return ResourceManager.GetString("MissingNameAttributeDescription", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to A &apos;{0}&apos; element requires a &apos;name&apos; attribute value..
        /// </summary>
        public static string MissingNameAttributeMessageFormat {
            get {
                return ResourceManager.GetString("MissingNameAttributeMessageFormat", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to COM-visible classes must have an explicit ProgId attribute. This is required to avoid verisoning problems. Refer to RubberduckProgId constants..
        /// </summary>
        public static string MissingProgIdDescription {
            get {
                return ResourceManager.GetString("MissingProgIdDescription", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to MissingProgId.
        /// </summary>
        public static string MissingProgIdId {
            get {
                return ResourceManager.GetString("MissingProgIdId", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to COM-visible class &apos;{0}&apos; does not have an explicit ProgId attribute that references a RubberduckProgId constant..
        /// </summary>
        public static string MissingProgIdMessageFormat {
            get {
                return ResourceManager.GetString("MissingProgIdMessageFormat", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Missing ProgId Attribute.
        /// </summary>
        public static string MissingProgIdTitle {
            get {
                return ResourceManager.GetString("MissingProgIdTitle", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Missing &apos;RequiredLibrary&apos; attribute.
        /// </summary>
        public static string MissingRequiredLibAttribute {
            get {
                return ResourceManager.GetString("MissingRequiredLibAttribute", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The &lt;reference name=&quot;RequiredLibrary&quot; /&gt; tag means to document the presence of a [RequiredLibraryAttribute]. If the attribute is correctly missing, the xml-doc tag should be removed..
        /// </summary>
        public static string MissingRequiredLibAttributeDescription {
            get {
                return ResourceManager.GetString("MissingRequiredLibAttributeDescription", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to XML documentation of type &apos;{0}&apos; includes a &lt;reference&gt; tag, but no corresponding [RequiredLibraryAttribute] is decorating the inspection type. Expected: [RequiredLibrary(&quot;{1}&quot;)]..
        /// </summary>
        public static string MissingRequiredLibAttributeMessageFormat {
            get {
                return ResourceManager.GetString("MissingRequiredLibAttributeMessageFormat", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Project website compatibility.
        /// </summary>
        public static string XmlDocAnalyzerCategory {
            get {
                return ResourceManager.GetString("XmlDocAnalyzerCategory", resourceCulture);
            }
        }
    }
}
