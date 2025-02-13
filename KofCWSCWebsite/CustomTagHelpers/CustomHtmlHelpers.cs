
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Linq.Expressions;


using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;

using ModelMetadata = System.Web.Mvc.ModelMetadata;
using TagBuilder = System.Web.Mvc.TagBuilder;






namespace KofCWSCWebsite.CustomTagHelpers
{

    public static class CustomHtmlHelpers
    {
        public static IHtmlContent CustomHiddenFor<TModel, TProperty>(this IHtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, object htmlAttributes = null)
        {
            var builder = new HtmlContentBuilder();
            builder.AppendHtml($"<input \"");

            foreach (var attribute in htmlAttributes.GetType().GetProperties())
            {
                var myname = attribute.Name;
                var value = attribute.GetValue(htmlAttributes);
                builder.AppendHtml($" {myname}=\"{value}\"");
            }

            var name = ExpressionHelper.GetExpressionText(expression);
            builder.AppendHtml($" name=\"{name}\"");

            var modelValue = expression.Compile().Invoke(htmlHelper.ViewData.Model);
            builder.AppendHtml($" value=\"false\"");

            builder.AppendHtml($" type=\"hidden\"");

            

            builder.AppendHtml("/>");

            return builder;
        }
        public static IHtmlContent CustomCheckBoxFor<TModel, TProperty>(this IHtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, object htmlAttributes = null)
        {
            var builder = new HtmlContentBuilder();
            builder.AppendHtml($"<input type=\"checkbox\"");

            foreach (var attribute in htmlAttributes.GetType().GetProperties())
            {
                var myname = attribute.Name;
                var value = attribute.GetValue(htmlAttributes);
                builder.AppendHtml($" {myname}=\"{value}\"");
            }

            var name = ExpressionHelper.GetExpressionText(expression);
            builder.AppendHtml($" name=\"{name}\"");

            var modelValue = expression.Compile().Invoke(htmlHelper.ViewData.Model);
            builder.AppendHtml($" value=\"{modelValue}\"");

            if (modelValue.ToString() == "True")
            {
                builder.AppendHtml(" checked=\"checked\"");
            }

            builder.AppendHtml("/>");

            return builder;
        }

        public static IHtmlContent CustomTextBoxFor<TModel, TProperty>(this IHtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, object htmlAttributes = null)
        {
            var builder = new HtmlContentBuilder();
            builder.AppendHtml($"<input type=\"textbox\"");

            foreach (var attribute in htmlAttributes.GetType().GetProperties())
            {
                var myname = attribute.Name;
                var value = attribute.GetValue(htmlAttributes);
                builder.AppendHtml($" {myname}=\"{value}\"");
            }

            var name = ExpressionHelper.GetExpressionText(expression);
            builder.AppendHtml($" name=\"{name}\"");

            var modelValue = expression.Compile().Invoke(htmlHelper.ViewData.Model);
            builder.AppendHtml($" value=\"{modelValue}\"");

            if (modelValue.ToString() == "True")
            {
                builder.AppendHtml(" checked=\"checked\"");
            }

            builder.AppendHtml("/>");

            return builder;
        }

        //public static MvcHtmlString CheckBoxFor<TModel, TValue>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TValue>> expression)
        //{
        //    var metadata = ModelMetadata.FromLambdaExpression(expression, htmlHelper.ViewData);
        //    var name = ExpressionHelper.GetExpressionText(expression);
        //    var fullHtmlFieldName = htmlHelper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(name);
        //    var checkbox = new TagBuilder("input");
        //    checkbox.Attributes["type"] = "checkbox";
        //    checkbox.Attributes["name"] = fullHtmlFieldName;
        //    checkbox.Attributes["id"] = fullHtmlFieldName;
        //    checkbox.Attributes["value"] = "true";
        //    checkbox.Attributes["checked"] = metadata.Model == true ? "checked" : null;

        //    var hiddenInput = new TagBuilder("input");
        //    hiddenInput.Attributes["type"] = "hidden";
        //    hiddenInput.Attributes["name"] = fullHtmlFieldName;
        //    hiddenInput.Attributes["value"] = "false";

        //    var label = new TagBuilder("label");
        //    label.Attributes["for"] = fullHtmlFieldName;
        //    label.InnerHtml = htmlHelper.LabelFor(expression).ToHtmlString();

        //    var div = new TagBuilder("div");
        //    div.InnerHtml = checkbox.ToString() + hiddenInput.ToString() + label.ToString();

        //    return MvcHtmlString.Create(div.ToString());
        //}



    }
}





