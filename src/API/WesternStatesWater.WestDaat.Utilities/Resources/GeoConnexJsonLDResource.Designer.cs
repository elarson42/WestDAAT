﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace WesternStatesWater.WestDaat.Utilities.Resources {
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
    internal class GeoConnexJsonLDResource {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal GeoConnexJsonLDResource() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("WesternStatesWater.WestDaat.Utilities.Resources.GeoConnexJsonLDResource", typeof(GeoConnexJsonLDResource).Assembly);
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
        ///   Looks up a localized string similar to Water rights data downloaded from Western States Water Data Access and Analysis Tool (WestDAAT) on [insert download date here]
        ///
        ///By WaDE2 Staff
        ///Adel Abdallah, WaDE Program Manager
        ///Email: adelabdallah@wswc.utah.gov
        ///Ryan James, Data Analyst / Hydroinformatics Specialist
        ///Email: rjames@wswc.utah.gov
        ///
        ///How to cite this data?
        ///Adel Abdallah and Ryan James. Western States Water Data Access and Analysis Tool (WestDAAT), Western States Water Council, [insert download date here]
        ///
        ///DISCLAIMER and Terms of Servi [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string Citation {
            get {
                return ResourceManager.GetString("Citation", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {{
        ///  &quot;@context&quot;: [
        ///    {{
        ///      &quot;schema&quot;: &quot;http://schema.org/&quot;,
        ///      &quot;skos&quot;: &quot;https://www.w3.org/TR/skos-reference/&quot;,
        ///      &quot;sosa&quot;: &quot;http://www.w3.org/ns/sosa/&quot;,
        ///      &quot;hyf&quot;: &quot;https://www.opengis.net/def/schema/hy_features/hyf/&quot;,
        ///      &quot;gsp&quot;: &quot;http://www.opengeospatial.org/standards/geosparql/&quot;,
        ///      &quot;name&quot;: &quot;schema:name&quot;,
        ///      &quot;sameAs&quot;: &quot;schema:sameAs&quot;,
        ///      &quot;related&quot;: &quot;skos:related&quot;,
        ///      &quot;description&quot;: &quot;schema:description&quot;,
        ///      &quot;geo&quot;: &quot;schema:geo&quot;,
        ///      &quot;image&quot;: {{
        ///        &quot;@id&quot;: &quot;s [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string GeoConnexJsonLDTemplate {
            get {
                return ResourceManager.GetString("GeoConnexJsonLDTemplate", resourceCulture);
            }
        }
    }
}
