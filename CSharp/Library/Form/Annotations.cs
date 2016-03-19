﻿using System;
using System.Linq;

namespace Microsoft.Bot.Builder.Form
{
    /// <summary>
    /// Attribute to override the default description of a field, property or enum value.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Enum | AttributeTargets.Property)]
    public class Describe : Attribute
    {
        public readonly string Description;

        /// <summary>
        /// Description for field, property or enum value.
        /// </summary>
        /// <param name="description"></param>
        public Describe(string description)
        {
            Description = description;
        }
    }

    /// <summary>
    /// Attribute to override the default terms used to match a field, property or enum value to user input.
    /// </summary>
    /// <remarks>
    /// By default terms are generated by calling the <see cref="Advanced.Language.GenerateTerms(string, int)"/> method with a max phrase length of 3
    /// on the name of the field, property or enum value.  Using this attribute you can specify your own regular expressions to match or if you specify the 
    /// <see cref="MaxPhrase"/> attribute you can cause <see cref="Advanced.Language.GenerateTerms(string, int)"/> to be called on your strings with the
    /// maximum phrase length you specify.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Enum | AttributeTargets.Property)]
    public class Terms : Attribute
    {
        /// <summary>
        /// Regular expressions for matching user input.
        /// </summary>
        public string[] Alternatives;

        private int _maxPhrase;
        /// <summary>
        /// The maximum pharse length to use when calling <see cref="Advanced.Language.GenerateTerms(string, int)"/> on your supplied terms.
        /// </summary>
        public int MaxPhrase
        {
            get
            {
                return _maxPhrase;
            }
            set
            {
                _maxPhrase = value;
                Alternatives = Alternatives.SelectMany(alt => Advanced.Language.GenerateTerms(alt, _maxPhrase)).ToArray();
            }
        }

        /// <summary>
        /// Regular expressions or terms used when matching user input.
        /// </summary>
        /// <remarks>
        /// If <see cref="MaxPhrase"/> is specified the supplied alternatives will be passed to <see cref="Advanced.Language.GenerateTerms(string, int)"/> to generate regular expressions
        /// with a maximum phrase size of <see cref="MaxPhrase"/>.
        /// </remarks>
        /// <param name="alternatives">Regular expressions or terms.</param>
        public Terms(string[] alternatives)
        {
            Alternatives = alternatives;
        }

        /// <summary>
        /// Regular expression or terms to use when matching user input.
        /// </summary>
        /// <remarks>
        /// If <see cref="MaxPhrase"/> is specified the supplied alternatives will be passed to <see cref="Advanced.Language.GenerateTerms(string, int)"/> to generate regular expressions
        /// with a maximum phrase size of <see cref="MaxPhrase"/>.
        /// </remarks>
        /// <param name="root">A single regular expression or term.</param>
        public Terms(string root)
        {
            Alternatives = new string[] { root };
        }
    }

    /// <summary>
    /// Specifies how to show choices generated by {||} in a \ref patterns string.
    /// </summary>
    public enum ChoiceStyleOptions {
        /// <summary>
        /// Use the default <see cref="ChoiceStyle"/> from the <see cref="FormConfiguration.DefaultPrompt"/>.
        /// </summary>
        Default,

        /// <summary>
        /// Automatically switch between the <see cref="Inline"/> and <see cref="PerLine"/> styles based on the number of choices.
        /// </summary>
        Auto,

        /// <summary>
        /// Show choices on the same line.
        /// </summary>
        Inline,

        /// <summary>
        /// Show choices with one per line.
        /// </summary>
        PerLine };


    /// <summary>
    /// How to normalize the case of words.
    /// </summary>
    public enum CaseNormalization {

        /// <summary>
        /// Use the default from the <see cref="FormConfiguration.DefaultPrompt"/>.
        /// </summary>
        Default,

        /// <summary>
        /// First letter of each word is capitalized
        /// </summary>
        InitialUpper, 

        /// <summary>
        /// Normalize words to lower case.
        /// </summary>
        Lower,

        /// <summary>
        /// Normalize words to upper case.
        /// </summary>
        Upper,

        /// <summary>
        /// Don't normalize words.
        /// </summary>
        None };

    /// <summary>
    /// Three state boolean value.
    /// </summary>
    public enum BoolDefault {
        /// <summary>
        /// Use the default from the <see cref="FormConfiguration.DefaultPrompt"/>.
        /// </summary>
        Default,

        /// <summary>
        /// Boolean true.
        /// </summary>
        Yes,

        /// <summary>
        /// Boolean false.
        /// </summary>
        No };

    /// <summary>
    /// Control how the user gets feedback after each entry.
    /// </summary>
    public enum FeedbackOptions {
        /// <summary>
        /// Use the default from the <see cref="FormConfiguration.DefaultPrompt"/>.
        /// </summary>
        Default,

        /// <summary>
        /// Provide feedback using the <see cref="TemplateUsage.Feedback"/> template only if part of the user input was not understood.
        /// </summary>
        Auto,

        /// <summary>
        /// Provide feedback after every user input.
        /// </summary>
        Always,

        /// <summary>
        /// Never provide feedback.
        /// </summary>
        Never };

    /// <summary>
    /// Abstract base class used by all attributes that use \ref patterns.
    /// </summary>
    public abstract class TemplateBase : Attribute
    {
        private readonly string[] _patterns;
        private static Random _generator = new Random();

        /// <summary>
        /// When processing choices {||} in a \ref patterns string, provide a choice for the default value if present.
        /// </summary>
        public BoolDefault AllowDefault { get; set; }

        /// <summary>
        /// Allow matching on numbers.
        /// </summary>
        public BoolDefault AllowNumbers { get; set; }

        /// <summary>
        /// Control case when showing {&} field name references in a \ref patterns string.
        /// </summary>
        public CaseNormalization FieldCase { get; set; }

        /// <summary>
        /// Control case when showing {} value references in a \ref patterns string.
        /// </summary>
        public CaseNormalization ValueCase { get; set; }

        /// <summary>
        /// Control what kind of feedback the user gets after each input.
        /// </summary>
        public FeedbackOptions Feedback { get; set; }

        /// <summary>
        /// Format string used for presenting each choice when showing {||} choices in a \ref patterns string.
        /// </summary>
        /// <remarks>The choice format is passed two arguments, {0} is the number of the choice and {1} is the field name.</remarks>
        public string ChoiceFormat { get; set; }

        /// <summary>
        /// When constructing lists using {[]} in a \ref patterns string, the string used before the last value in the list.
        /// </summary>
        public string LastSeparator { get; set; }

        /// <summary>
        /// When constructing lists using {[]} in a \ref patterns string, the string used between all values except the last.
        /// </summary>
        public string Separator { get; set; }

        /// <summary>
        /// How to display choices {||} when processed in a \ref patterns string.
        /// </summary>
        public ChoiceStyleOptions ChoiceStyle { get; set; }

        /// <summary>
        /// The pattern to use when generating a string using <see cref="Advanced.IPrompt{T}"/>.
        /// </summary>
        /// <remarks>If multiple patterns were specified, then each call to this function will return a random pattern.</remarks>
        /// <returns>Pattern to use.</returns>
        public string Pattern()
        {
            var choice = 0;
            if (_patterns.Length > 1)
            {
                choice = _generator.Next(_patterns.Length);
            }
            return _patterns[choice];
        }

        /// <summary>
        /// All possible templates.
        /// </summary>
        /// <returns>The possible templates.</returns>
        public string[] Patterns()
        {
            return _patterns;
        }

        /// <summary>
        /// Any default values in this template will be overridden by the supplied <see cref="defaultTemplate"/>.
        /// </summary>
        /// <param name="defaultTemplate">Default template to use to override default values.</param>
        public void ApplyDefaults(TemplateBase defaultTemplate)
        {
            if (AllowDefault == BoolDefault.Default) AllowDefault = defaultTemplate.AllowDefault;
            if (AllowNumbers == BoolDefault.Default) AllowNumbers = defaultTemplate.AllowNumbers;
            if (ChoiceStyle == ChoiceStyleOptions.Default) ChoiceStyle = defaultTemplate.ChoiceStyle;
            if (FieldCase == CaseNormalization.Default) FieldCase = defaultTemplate.FieldCase;
            if (Feedback == FeedbackOptions.Default) Feedback = defaultTemplate.Feedback;
            if (ChoiceFormat == null) ChoiceFormat = defaultTemplate.ChoiceFormat;
            if (LastSeparator == null) LastSeparator = defaultTemplate.LastSeparator;
            if (Separator == null) Separator = defaultTemplate.Separator;
            if (ValueCase == CaseNormalization.Default) ValueCase = defaultTemplate.ValueCase;
        }

        /// <summary>
        /// Initialize with a single template.
        /// </summary>
        /// <param name="pattern">Pattern to use.</param>
        public TemplateBase(string pattern)
        {
            _patterns = new string[] { pattern};
            Initialize();
        }

        /// <summary>
        /// Initialize with multiple patterns that will be chosen from randomly.
        /// </summary>
        /// <param name="patterns">Possible patterns.</param>
        public TemplateBase(string[] patterns)
        {
            _patterns = patterns;
            Initialize();
        }

        /// <summary>
        /// Initialize from another template.
        /// </summary>
        /// <param name="other">The template to copy from.</param>
        public TemplateBase(TemplateBase other)
        {
            _patterns = other._patterns;
            AllowDefault = other.AllowDefault;
            AllowNumbers = other.AllowNumbers;
            ChoiceStyle = other.ChoiceStyle;
            FieldCase = other.FieldCase;
            Feedback = other.Feedback;
            ChoiceFormat = other.ChoiceFormat;
            LastSeparator = other.LastSeparator;
            Separator = other.Separator;
            ValueCase = other.ValueCase;
        }

        private void Initialize()
        {
            AllowDefault = BoolDefault.Default;
            AllowNumbers = BoolDefault.Default;
            ChoiceStyle = ChoiceStyleOptions.Default;
            FieldCase = CaseNormalization.Default;
            Feedback = FeedbackOptions.Default;
            ChoiceFormat = null;
            LastSeparator = null;
            Separator = null;
            ValueCase = CaseNormalization.Default;
        }
    }

    /// <summary>
    /// Define the prompt template used when asking about a field.
    /// </summary>
    /// <remarks>
    /// Prompts use \ref Templates to provide control over what goes into a prompt.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class Prompt : TemplateBase
    {
        /// <summary>
        /// Define prompt from a single template.
        /// </summary>
        /// <param name="pattern">Template to use.</param>
        public Prompt(string pattern)
            : base(pattern)
        {
        }

        /// <summary>
        /// Define a prompt with multiple templates that will be selected from randomly.
        /// </summary>
        /// <param name="patterns"></param>
        public Prompt(string[] patterns)
            : base(patterns)
        { }

        /// <summary>
        /// Define a prompt based on a <see cref="Template"/>.
        /// </summary>
        /// <param name="pattern">Template to use.</param>
        public Prompt(Template pattern)
            : base(pattern)
        {
        }
    }

    /// <summary>
    /// All of the built-in templates.
    /// </summary>
    /// <remarks>
    /// A good way to understand these is to look at the default templates defined in <see cref="FormConfiguration.Templates"/>
    /// </remarks>
    public enum TemplateUsage
    {
        /// <summary>
        /// Clarify an ambiguous choice.
        /// </summary>
        /// <remarks>This template can use {0} to capture the term that was ambiguous.</remarks>
        Clarify,

        /// <summary>
        /// Show the current choice.
        /// </summary>
        /// <remarks>
        /// This is how the current choice is represented as an option.  If you change this, you should also change <see cref="FormConfiguration.CurrentChoice"/>
        /// so that what people can type matches what you show.
        /// </remarks>
        CurrentChoice,

        /// <summary>
        /// How to ask for a <see cref="DateTime"/>.
        /// </summary>
        DateTime,

        /// <summary>
        /// How to ask for a double.
        /// </summary>
        Double,

        /// <summary>
        /// How to show feedback after user input.
        /// </summary>
        /// <remarks>
        /// Unmatched input is available through {0}, but it should be wrapped in an optional {?} in \ref patterns in case everything was matched. 
        /// </remarks>
        Feedback,

        /// <summary>
        /// What to display when asked for help.
        /// </summary>
        /// <remarks>
        /// This template controls the overall help experience.  {0} will be recognizer specific help and {1} will be command help.
        /// </remarks>
        Help,

        /// <summary>
        /// What you can enter when clarifying. 
        /// </summary>
        HelpClarify,

        /// <summary>
        /// What for can enter when entering a <see cref="DateTime"/>.
        /// </summary>
        HelpDateTime,

        /// <summary>
        /// What you can enter when entering a double.
        /// </summary>
        HelpDouble,

        /// <summary>
        /// What you can enter while entering an integer.
        /// </summary>
        HelpInteger,

        /// <summary>
        /// What you can enter while navigating.
        /// </summary>
        HelpNavigation,

        /// <summary>
        /// What you can enter when selecting a single value from a numbered enumeration.
        /// </summary>
        HelpOneNumber,

        /// <summary>
        ///  What you can enter when selecting multiple values from a numbered enumeration.
        /// </summary>
        HelpManyNumber,

        /// <summary>
        /// What you can enter when selecting one value from an enumeration.
        /// </summary>
        HelpOneWord,

        /// <summary>
        /// What you can enter when selecting mutiple values from an enumeration.
        /// </summary>
        HelpManyWord,

        /// <summary>
        /// What you can enter when entering a string.
        /// </summary>
        HelpString,

        /// <summary>
        /// How to ask for an integer.
        /// </summary>
        Integer,

        /// <summary>
        /// How to ask for a navigation.
        /// </summary>
        Navigation,

        /// <summary>
        /// How to represent no value in an optional field. 
        /// </summary>
        NoPreference,

        /// <summary>
        /// Response when an input is not understood.
        /// </summary>
        /// <remarks>
        /// When no input is matched this template is used and gets {0} for what the user entered.</remarks>
        NotUnderstood,

        /// <summary>
        /// How to ask for one value from an enumeration.
        /// </summary>
        SelectOne,

        /// <summary>
        /// How to ask for multiple values from an enumeration.
        /// </summary>
        SelectMany,

        /// <summary>
        /// How to ask for a string.
        /// </summary>
        String,

        /// <summary>
        /// How to represent a value that has not yet been specified.
        /// </summary>
        Unspecified
    };

    /// <summary>
    /// Define a template for generating strings.
    /// </summary>
    /// <remarks>
    /// Templates provide a pattern that uses the template language defined in \ref patterns.  See <see cref="TemplateUsage"/> to see a description of all the different kinds of templates.
    /// You can also look at <see cref="FormConfiguration.Templates"/> to see all the default templates that are provided.  Templates can be overriden at the form, class/struct of field level.  
    /// They also support randomly selecting between templates which is a good way to introduce some variation in your responses.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = true)]
    public class Template : TemplateBase
    {
        /// <summary>
        /// What kind of template this is.
        /// </summary>
        public readonly TemplateUsage Usage;

        /// <summary>
        /// Specify a template for a particular usage.
        /// </summary>
        /// <param name="usage">How the template will be used.</param>
        /// <param name="pattern">The text pattern for the template.</param>
        public Template(TemplateUsage usage, string pattern)
            : base(pattern)
        {
            Usage = usage;
        }

        /// <summary>
        /// Specify a set of templates to randomly choose between for a particular usage.
        /// </summary>
        /// <param name="usage">How the template will be used.</param>
        /// <param name="patterns">The set of \ref patterns to randomly choose from.</param>
        public Template(TemplateUsage usage, string[] patterns)
            : base(patterns)
        {
            Usage = usage;
        }
    }

    /// <summary>
    /// Define a field or property as optional.
    /// </summary>
    /// <remarks>
    /// An optional field is one where having no value is an acceptable response.  By default every field is considered required and must be filled in to complete the form.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class Optional : Attribute
    {
        /// <summary>
        /// Mark a field or property as optional.
        /// </summary>
        public Optional()
        { }
    }

    /// <summary>
    /// Provide limits on the possible values in a numeric field or property.
    /// </summary>
    /// <remarks>
    /// By default the limits are the min and max of the underlying field type.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class Numeric : Attribute
    {
        /// <summary>
        /// Min possible value.
        /// </summary>
        public readonly double Min;

        /// <summary>
        /// Max possible value.
        /// </summary>
        public readonly double Max;

        /// <summary>
        /// Specify the range of possible values for a number field.
        /// </summary>
        /// <param name="min">Min value allowed.</param>
        /// <param name="max">Max value allowed.</param>
        public Numeric(double min, double max)
        {
            Min = min;
            Max = max;
        }
    }
}
