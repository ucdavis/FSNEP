using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Web.Mvc;
using MvcContrib.FluentHtml;
using MvcContrib.FluentHtml.Behaviors;
using MvcContrib.FluentHtml.Elements;
using MvcContrib.FluentHtml.Html;

namespace FSNEP.Helpers
{
    /// <summary>
    /// Like a checkBoxList but is rendered as an unordered list, with each checkbox in an li
    /// </summary>
    public class CheckBoxListUnordered : CheckBoxListBase<CheckBoxListUnordered>
    {
        public CheckBoxListUnordered(string name, MemberExpression forMember, IEnumerable<IBehaviorMarker> behaviors) : base(HtmlTag.UnOrderedList, name, forMember, behaviors)
        {
        }

        public CheckBoxListUnordered(string name) : base(HtmlTag.UnOrderedList, name)
        {
        }

        protected override void PreRender()
        {
            builder.InnerHtml = RenderBody();
        }

        
        private string RenderBody()
        {
            if (_options == null)
            {
                return null;
            }

            var name = builder.Attributes[HtmlAttribute.Name];
            builder.Attributes.Remove(HtmlAttribute.Name);
            var sb = new StringBuilder();
            var i = 0;
            foreach (var option in _options)
            {
                var liBuilder = new TagBuilder(HtmlTag.ListItem);
                
                var value = _valueFieldSelector(option);
                var checkbox = (new CheckBox(name, forMember, behaviors)
                    .Id(string.Format("{0}_{1}", name.FormatAsHtmlId(), i))
                    .Value(value))
                    .LabelAfter(_textFieldSelector(option).ToString(), _itemClass)
                    .Checked(IsSelectedValue(value));
                if (_itemClass != null)
                {
                    checkbox.Class(_itemClass);
                }

                liBuilder.InnerHtml =_itemFormat == null
                                         ? checkbox.ToCheckBoxOnlyHtml()
                                         : string.Format(_itemFormat, checkbox.ToCheckBoxOnlyHtml());
                i++;

                sb.Append(liBuilder.ToString());
            }
            return sb.ToString();
        }
        
    }
}