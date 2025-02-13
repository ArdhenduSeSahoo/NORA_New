//Specific text providers for framework functionality
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Damco.Model
{
    /// <summary>
    /// RootTextProvider class returns specific TextProvider based on the input. 
    /// </summary>
    /// <remarks>
    /// Ex. ApplicationInfoTextProvider, GeneralTextProvider, LayoutTextProvider, DatePickerTextProvider etc. 
    /// </remarks>
    public partial class RootTextProvider { public virtual ApplicationInfoTextProvider ApplicationInfo { get { return GetProvider<ApplicationInfoTextProvider>(); } }    }

    /// <summary>
    /// Maintains ApplicationInfo.
    /// </summary>
    /// <remarks>
    /// Like ApplicationCode and ApplicationName using ApplicationInfoTextProvider.
    /// </remarks>
    public class ApplicationInfoTextProvider : MessageTextProvider
    {
        /// <summary>
        /// Returns Code of the Application.
        /// </summary>
        public virtual string ShortName { get { return this.GetText(ConfigurationManager.AppSettings["ApplicationShortName"] ?? this.GetType().FullName.Substring(0, this.GetType().FullName.IndexOf("."))); } }

        /// <summary>
        /// Returns Name of the Application.
        /// </summary>
        public virtual string Name { get { return this.GetText(ConfigurationManager.AppSettings["ApplicationName"] ?? this.GetType().FullName.Substring(0, this.GetType().FullName.IndexOf("."))); } }
    }

    public partial class RootTextProvider { public virtual GeneralTextProvider General { get { return GetProvider<GeneralTextProvider>(); } } }

    /// <summary>
    /// Maintains details about the text. 
    /// </summary>
    public class GeneralTextProvider : MessageTextProvider
    {
        /// <summary>
        /// Returns Apply as the default text.
        /// </summary>
        public string Apply { get { return this.GetDefaultText(); } }

        /// <summary>
        /// Returns blank as the default text. 
        /// </summary>
        public string Blank { get { return this.GetText("< blank >"); } }

        /// <summary>
        /// Returns OK as the default text.
        /// </summary>
        public string OK { get { return this.GetDefaultText(); } }
    }

    public partial class RootTextProvider { public virtual LayoutTextProvider Layout { get { return GetProvider<LayoutTextProvider>(); } } }

    /// <summary>
    /// Maintains details about the layout content.
    /// </summary>
    public class LayoutTextProvider : MessageTextProvider
    {
        /// <summary>
        /// UserCode of the Logged in User details is set in the layout.
        /// </summary>
        /// <param name="userCode">User who is logged.</param>
        /// <returns>Logged in user details.</returns>
        public virtual string UserCodeInHeader(string userCode) { return this.GetText("User: {0}", Params(userCode)); }

        /// <summary>
        /// Manage text to provide Manage options for the logged in user is set in the layout.
        /// </summary>
        public string Manage { get { return this.GetText("Manage"); } }

        /// <summary>
        /// SignOut text to log out the current user.
        /// </summary>
        public string SignOut { get { return this.GetText("SIGN OUT"); } }
    }

    [Serializable()]
    public enum MonthOfYear
    {
        January = 1,
        February = 2,
        March = 3,
        April = 4,
        May = 5,
        June = 6,
        July = 7,
        August = 8,
        September = 9,
        October = 10,
        November = 11,
        December = 12
    }

    public partial class RootTextProvider { public virtual DatePickerTextProvider DatePicker { get { return GetProvider<DatePickerTextProvider>(); } } }
    /// <summary>
    /// Maintains details about the Day/Month details of the DatePicker control.
    /// </summary>
    public class DatePickerTextProvider : MessageTextProvider
    {
        /// <summary>
        /// Today text in the DatePicker is set to Today.
        /// </summary>
        public string Today { get { return this.GetDefaultText(); } }

        /// <summary>
        /// Done text in the DatePicker is set to Done.
        /// </summary>
        public string Done { get { return this.GetDefaultText(); } }

        /// <summary>
        /// Previous text in the DatePicker is set to Prev.
        /// </summary>
        public string Previous { get { return this.GetText("Prev"); } }

        /// <summary>
        /// Next text in the DatePicker is set to Next.
        /// </summary>
        public string Next { get { return this.GetDefaultText(); } }

        /// <summary>
        /// Week text in the DatePicker is set to wk.
        /// </summary>
        public string ShortWeek { get { return this.GetText("wk"); } }

        /// <summary>
        /// Day name is set to day of the week.
        /// </summary>
        /// <param name="dayOfWeek">Day of the week.</param>
        /// <returns>Day name of the week.</returns>
        public string DayName(DayOfWeek dayOfWeek) { return this.GetDefaultTextForEnum(dayOfWeek); }

        /// <summary>
        /// Day name in short.
        /// </summary>
        /// <param name="dayOfWeek">day of the week.</param>
        /// <returns>Short name of the day of the week.</returns>
        public string DayNameShort(DayOfWeek dayOfWeek)
        {
            return this.GetTextForEnum(
                dayOfWeek,
                    dayOfWeek == DayOfWeek.Monday ? "Mon"
                    : dayOfWeek == DayOfWeek.Tuesday ? "Tue"
                    : dayOfWeek == DayOfWeek.Wednesday ? "Wed"
                    : dayOfWeek == DayOfWeek.Thursday ? "Thu"
                    : dayOfWeek == DayOfWeek.Friday ? "Fri"
                    : dayOfWeek == DayOfWeek.Saturday ? "Sat"
                    : dayOfWeek == DayOfWeek.Sunday ? "Sun"
                    : null
                );
        }

        /// <summary>
        /// Day name in extra short.
        /// </summary>
        /// <param name="dayOfWeek">Day of the week.</param>
        /// <returns>Extra short day name.</returns>
        public string DayNameExtraShort(DayOfWeek dayOfWeek)
        {
            return this.GetTextForEnum(
                dayOfWeek,
                    dayOfWeek == DayOfWeek.Monday ? "Mo"
                    : dayOfWeek == DayOfWeek.Tuesday ? "Tu"
                    : dayOfWeek == DayOfWeek.Wednesday ? "We"
                    : dayOfWeek == DayOfWeek.Thursday ? "Th"
                    : dayOfWeek == DayOfWeek.Friday ? "Fr"
                    : dayOfWeek == DayOfWeek.Saturday ? "Sa"
                    : dayOfWeek == DayOfWeek.Sunday ? "Su"
                    : null
                );
        }

        /// <summary>
        /// Month name of the year.
        /// </summary>
        /// <param name="monthOfYear">Month of the year.</param>
        /// <returns>Month of the year.</returns>
        public string MonthName(MonthOfYear monthOfYear) { return this.GetDefaultTextForEnum(monthOfYear); }

        /// <summary>
        /// Month name in short.
        /// </summary>
        /// <param name="monthOfYear">Month of the year.</param>
        /// <returns>Short name of the month.</returns>
        public string MonthNameShort(MonthOfYear monthOfYear)
        {
            return this.GetTextForEnum(
                monthOfYear,
                    monthOfYear == MonthOfYear.January ? "Jan"
                    : monthOfYear == MonthOfYear.February ? "Feb"
                    : monthOfYear == MonthOfYear.March ? "Mar"
                    : monthOfYear == MonthOfYear.April ? "Apr"
                    : monthOfYear == MonthOfYear.May ? "May"
                    : monthOfYear == MonthOfYear.June ? "Jun"
                    : monthOfYear == MonthOfYear.July ? "Jul"
                    : monthOfYear == MonthOfYear.August ? "Aug"
                    : monthOfYear == MonthOfYear.September ? "Sep"
                    : monthOfYear == MonthOfYear.October ? "Oct"
                    : monthOfYear == MonthOfYear.November ? "Nov"
                    : monthOfYear == MonthOfYear.December ? "Dec"
                    : null
                );
        }
    }

    [Serializable()]
    public enum DataFilterOperator
    {
        Is,
        IsNot,
        MoreThan,
        MoreOrIs,
        LessThan,
        LessOrIs,
        IsBlank,
        IsNotBlank,
        StartsWith,
        StartsNotWith,
        EndsWith,
        EndsNotWith,
        Contains,
        ContainsNot,
        And,
        Or
    }

    public partial class RootTextProvider { public virtual DataFilterTextProvider DataFilter { get { return GetProvider<DataFilterTextProvider>(); } } }

    /// <summary>
    /// Maintains details about filtering the data.
    /// </summary>
    /// <remarks>
    /// For example - AND, OR, StartsWith etc.
    /// </remarks>
    public class DataFilterTextProvider : MessageTextProvider
    {
        /// <summary>
        /// AND text to be included in the filter.
        /// </summary>
        public string And { get { return this.GetText("and"); } }

        /// <summary>
        /// OR text to be included in the filter.
        /// </summary>
        public string Or { get { return this.GetText("or"); } }

        /// <summary>
        /// AND text to be included for an ANDButton.
        /// </summary>
        public string AndButton { get { return this.GetText("and"); } }

        /// <summary>
        /// OR text to be included for an ORButton.
        /// </summary>
        public string OrButton { get { return this.GetText("or"); } }

        /// <summary>
        /// Text to a button to add the first item of filters.
        /// </summary>
        public string AddFirstItemButton { get { return this.GetText("No filters in place. Click here to add the first."); } }

        /// <summary>
        /// Text for Data Filter operators.
        /// </summary>
        /// <param name="operator">DataFilter operator.</param>
        /// <returns>Text for Data Filter operators</returns>
        public string Operator(DataFilterOperator @operator)
        {
            return this.GetTextForEnum(@operator, TextProviderBase.PascalToDisplayText(@operator.ToString(), false).ToLower());
        }

        /// <summary>
        /// Text to the title as Filters for a dialog.
        /// </summary>
        public string DialogTitle { get { return this.GetText("Filters"); } }
    }

    public partial class RootTextProvider { public virtual AuthenticationTextProvider Authentication { get { return GetProvider<AuthenticationTextProvider>(); } } }

    /// <summary>
    /// Maintains details of the text required while authenticating the user.
    /// </summary>
    public class AuthenticationTextProvider : MessageTextProvider
    {
        /// <summary>
        /// Validation message text to Password field, generally while creating the password.
        /// </summary>
        /// <remarks>
        /// When there is not a single uppercase letter(A-Z) in the chosen password.
        /// </remarks>
        /// <returns>Validation message text.</returns>
        public string PasswordInvalidMustHaveUpperCase { get { return GetText("Passwords must have at least one uppercase ('A'-'Z')."); } }

        /// <summary>
        /// Validation message text to Password field, generally while creating the password.
        /// </summary>
        /// <remarks>
        /// When there is not a single lowercase letter(a-z) in the chosen password.
        /// </remarks>
        /// <returns>Validation message text.</returns>
        public string PasswordInvalidMustHaveLowerCase { get { return GetText("Passwords must have at least one lowercase ('a'-'z')."); } }

        /// <summary>
        /// Validation message text to Password field, generally while creating the password.
        /// </summary>
        /// <remarks>
        /// When there is not a single digit (0-9) in the chosen password.
        /// </remarks>
        /// <returns>Validation message text.</returns>
        public string PasswordInvalidMustHaveDigit { get { return GetText("Passwords must have at least one digit ('0'-'9')."); } }

        /// <summary>
        /// Validation message text to Password field, generally while creating the password.
        /// </summary>
        /// <remarks>
        /// When there is not a single non-letter or digit character in the chosen password.
        /// </remarks>
        /// <returns>Validation message text.</returns>
        public string PasswordInvalidMustHaveSymbol { get { return GetText("Passwords must have at least one non letter or digit character."); } }

        /// <summary>
        /// Validation message text to check if the chosen password has valid length to Password field.
        /// </summary>
        /// <param name="length">Valid length of the chosen password.</param>
        /// <returns>Validation message text.</returns>
        public string PasswordInvalidLength(int length) { return GetText("Passwords must be at least {0} characters.", Params(length.ToString())); }

        /// <summary>
        /// Validation message text indicating the credentials entered are incorrect.
        /// </summary>
        public string InvalidLoginAttempt { get { return GetText("Invalid login attempt."); } }

        /// <summary>
        /// Validation message text indicating password cannot be reset.
        /// </summary>
        public string CannotResetPassword { get { return GetText("You cannot reset the password for this account"); } }

        /// <summary>
        /// Text to be included in the subject of the Email to reset the password.
        /// </summary>
        public string ResetPasswordEmailSubject { get { return GetText("Reset password"); } }

        /// <summary>
        /// URL text to reset the password.
        /// </summary>
        /// <param name="callBackUrl"></param>
        /// <returns>Text to be in the Email and URL to reset the password.</returns>
        public string ResetPasswordEmailBody(string callBackUrl) { return GetText("Please reset your password by clicking <a href=\"{0}\">here</a>", Params(callBackUrl)); }

    }


    public partial class RootTextProvider { public virtual CombinerTextProvider Combine { get { return GetProvider<CombinerTextProvider>(); } } }

    /// <summary>
    /// Helps to combine group of text.
    /// </summary>
    public class CombinerTextProvider : MessageTextProvider
    {
        /// <summary>
        /// Text in a single line with double line gap from a list of steps of type TextPlaceHolder.
        /// </summary>
        /// <param name="steps">List of steps to be combined.</param>
        /// <returns>Each step in a single line with double line gap.</returns>
        public string ProcessSteps(IEnumerable<TextPlaceHolder> steps)
        {
            return GetText("{0}\n\n{0}", Params(steps));
        }

        /// <summary>
        /// Items to be placed in a single line with single line gap from a list of items of type TextPlaceHolder.
        /// </summary>
        /// <param name="items">List of items to be combined.</param>
        /// <returns>Items placed in a single line with single line gap</returns>
        public string ItemList(IEnumerable<TextPlaceHolder> items)
        {
            return GetText("{0}\n{0}", Params(items));
        }
    }

}
