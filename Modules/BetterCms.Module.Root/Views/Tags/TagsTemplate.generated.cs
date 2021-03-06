﻿#pragma warning disable 1591
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.34014
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace BetterCms.Module.Root.Views.Tags
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Text;
    using System.Web;
    using System.Web.Helpers;
    using System.Web.Mvc;
    using System.Web.Mvc.Ajax;
    using System.Web.Mvc.Html;
    using System.Web.Routing;
    using System.Web.Security;
    using System.Web.UI;
    using System.Web.WebPages;
    
    #line 1 "..\..\Views\Tags\TagsTemplate.cshtml"
    using BetterCms.Module.Root;
    
    #line default
    #line hidden
    
    #line 2 "..\..\Views\Tags\TagsTemplate.cshtml"
    using BetterCms.Module.Root.Content.Resources;
    
    #line default
    #line hidden
    
    #line 3 "..\..\Views\Tags\TagsTemplate.cshtml"
    using BetterCms.Module.Root.Mvc;
    
    #line default
    #line hidden
    
    #line 4 "..\..\Views\Tags\TagsTemplate.cshtml"
    using BetterCms.Module.Root.Mvc.Helpers;
    
    #line default
    #line hidden
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("RazorGenerator", "2.0.0.0")]
    [System.Web.WebPages.PageVirtualPathAttribute("~/Views/Tags/TagsTemplate.cshtml")]
    public partial class TagsTemplate : System.Web.Mvc.WebViewPage<BetterCms.Module.Root.ViewModels.Tags.TagsTemplateViewModel>
    {
        public TagsTemplate()
        {
        }
        public override void Execute()
        {
WriteLiteral("\r\n");

            
            #line 8 "..\..\Views\Tags\TagsTemplate.cshtml"
   var canEdit = (ViewContext.Controller as CmsControllerBase).SecurityService.IsAuthorized(RootModuleConstants.UserRoles.EditContent); 
            
            #line default
            #line hidden
WriteLiteral("\r\n<div");

WriteLiteral(" class=\"bcms-input-list-holder\"");

WriteLiteral(">\r\n");

WriteLiteral("    ");

            
            #line 10 "..\..\Views\Tags\TagsTemplate.cshtml"
Write(Html.Tooltip(Model.TooltipDescription));

            
            #line default
            #line hidden
WriteLiteral("\r\n    <div");

WriteLiteral(" class=\"bcms-content-titles\"");

WriteLiteral(">\r\n");

WriteLiteral("        ");

            
            #line 12 "..\..\Views\Tags\TagsTemplate.cshtml"
   Write(RootGlobalization.TagsTemplate_AddTags_Title);

            
            #line default
            #line hidden
WriteLiteral("\r\n        <div");

WriteLiteral(" class=\"bcms-btn-plus\"");

WriteLiteral(" data-bind=\"css: { \'bcms-btn-plus-expand\': isExpanded() }");

            
            #line 13 "..\..\Views\Tags\TagsTemplate.cshtml"
                                                                                       Write(canEdit ? ", click: expandCollapse" : string.Empty);

            
            #line default
            #line hidden
WriteLiteral("\"");

WriteLiteral(">&nbsp;</div>\r\n        <div");

WriteLiteral(" class=\"bcms-tags-field-holder\"");

WriteLiteral(" data-bind=\"visible: isExpanded()\"");

WriteLiteral(">\r\n            <input");

WriteLiteral(" type=\"text\"");

WriteLiteral(" class=\"bcms-add-tags-field\"");

WriteLiteral(" style=\"width: 107px;\"");

WriteLiteral(@" data-bind=""
                    css: { 'bcms-tag-validation-error': newItem.hasError() },
                    value: newItem,
                    valueUpdate: 'afterkeydown',
                    hasfocus: hasfocus,
                    autocompleteList: '',
                    enterPress: addItem,
                    escPress: clearItem""");

WriteLiteral(" />\r\n            <!-- ko if: newItem.hasError()  -->\r\n            <span");

WriteLiteral(" class=\"bcms-tag-field-validation-error\"");

WriteLiteral(">\r\n                <span");

WriteLiteral(" data-bind=\"text: newItem.validationMessage()\"");

WriteLiteral("></span>\r\n            </span>\r\n            <!-- /ko -->\r\n        </div>\r\n    </di" +
"v>\r\n    <div");

WriteLiteral(" class=\"bcms-single-tag-holder\"");

WriteLiteral(" data-bind=\"foreach: items()\"");

WriteLiteral(">\r\n        <div");

WriteLiteral(" class=\"bcms-single-tag\"");

WriteLiteral(" data-bind=\"css: { \'bcms-single-tag-active\': isActive() }\"");

WriteLiteral("><span");

WriteLiteral(" data-bind=\"    text: name()\"");

WriteLiteral("></span><a");

WriteLiteral(" data-bind=\"");

            
            #line 31 "..\..\Views\Tags\TagsTemplate.cshtml"
                                                                                                                                                   Write(canEdit ? "click: remove" : string.Empty);

            
            #line default
            #line hidden
WriteLiteral("\"");

WriteLiteral(">");

            
            #line 31 "..\..\Views\Tags\TagsTemplate.cshtml"
                                                                                                                                                                                               Write(RootGlobalization.Button_Remove);

            
            #line default
            #line hidden
WriteLiteral("</a></div>\r\n        <input");

WriteLiteral(" type=\"hidden\"");

WriteLiteral(" data-bind=\"attr: { name: getItemInputName($index()), value: name() }\"");

WriteLiteral(" />\r\n    </div>\r\n</div>\r\n");

        }
    }
}
#pragma warning restore 1591
