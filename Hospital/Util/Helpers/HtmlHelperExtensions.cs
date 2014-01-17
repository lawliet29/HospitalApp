using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace Hospital.Util.Helpers
{
    public static class HtmlHelperExtensions
    {
        private static MvcHtmlString FormGroupFor<TModel, TProperty>(
            this HtmlHelper<TModel> helper, Expression<Func<TModel, TProperty>> selector,
            bool isEditable,
            bool datepicker = false,
            string icon = null,
            bool textArea = false,
            string description = null,
            bool dateTimePicker = false) 
        {
            var sb = new StringBuilder();
            sb.Append("<div class='form-group'>");
            sb.Append(helper.LabelFor(selector, new { @class = "col-md-2 control-label" }).ToHtmlString());
            sb.Append("<div class='col-md-10'>");

            if (isEditable) 
            {
                sb.Append("<div class='right-inner-addon "+ (dateTimePicker ? "datetimepicker" : "") + "'>");
                if (!string.IsNullOrEmpty(icon))
                {
                    sb.Append("<i class='fa " + icon + "'></i>");
                }
                if (datepicker)
                {
                    sb.Append(helper.TextBoxFor(selector, "{0:dd.MM.yyyy}", new { @class = "form-control datepicker"}).ToHtmlString());
                }
                else if (textArea)
                {
                    sb.Append(helper.TextAreaFor(selector, new { @class = "form-control", rows = 5 }).ToHtmlString());
                }
                else if (dateTimePicker)
                {
                    sb.Append(helper.TextBoxFor(selector, "{0:dd.MM.yyyy hh:mm}", new { @class = "form-control" }).ToHtmlString());
                }
                else
                {
                    sb.Append(helper.TextBoxFor(selector, new { @class = "form-control" }).ToHtmlString());
                }
                sb.Append("</div>");
            }
            else 
            {
                sb.Append("<p class='form-control-static'>");
                string value = (datepicker
                    ? helper.ValueFor(selector, "{0:dd.MM.yyyy}")
                    : dateTimePicker 
                        ? helper.ValueFor(selector, "{0:dd.MM.yyyy hh:mm}")
                        : helper.ValueFor(selector)).ToHtmlString();

                sb.Append(string.IsNullOrWhiteSpace(value) ? "N/A" : value);
                sb.Append("</p>");
                sb.Append(helper.HiddenFor(selector, new { @class = "form-control disabled" }));
            
            }
            sb.Append(helper.ValidationMessageFor(selector));
            if (!string.IsNullOrEmpty(description))
            {
                sb.AppendFormat("<p class='help-block'>{0}</p>", description);
            }
            sb.Append("</div></div>");

            return new MvcHtmlString(sb.ToString());
        }

        public static MvcHtmlString FormGroupFor<TModel, TProperty>(this HtmlHelper<TModel> helper, Expression<Func<TModel, TProperty>> selector, bool isEditable, string icon = "")
        {
            return helper.FormGroupFor(selector, isEditable, false, icon);
        }

        public static MvcHtmlString FormGroupDatepickerFor<TModel, DateTime>(this HtmlHelper<TModel> helper, Expression<Func<TModel, DateTime>> selector, bool isEditable, string description = "")
        {
            return helper.FormGroupFor(selector, 
                isEditable: isEditable,
                datepicker: true, 
                icon: "fa-calendar", 
                description: description);
        }

        public static MvcHtmlString FormGroupDateTimePickerFor<TModel, DateTime>(this HtmlHelper<TModel> helper, Expression<Func<TModel, DateTime>> selector, bool isEditable, string description = "")
        {
            return helper.FormGroupFor(selector,
                isEditable: isEditable,
                dateTimePicker: true,
                icon: "fa-calendar",
                description: description);
        }

        public static MvcHtmlString FormGroupTextAreaFor<TModel, TProperty>(this HtmlHelper<TModel> helper, Expression<Func<TModel, TProperty>> selector, bool isEditable, string description)
        {
            return helper.FormGroupFor(selector, isEditable, textArea: true, description: description);
        }
    }
}