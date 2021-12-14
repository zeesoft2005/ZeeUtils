using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ZeeUtils
{
    public class ModelValidation<T>
    {
        List<ComponentModelValidationError> _errors = new List<ComponentModelValidationError>();
        public IReadOnlyCollection<ComponentModelValidationError> Errors { get { return _errors.AsReadOnly(); } }

        private int _expressionCount;
        private T _model;
        private bool _errorsByPriority=false;
        public ModelValidation(T model)
        {
            _model = model;
            _expressionCount = 0;
        }
        public ModelValidation(T model,bool showErrorsByPriority)
        {
            _model = model;
            _expressionCount = 0;
            _errorsByPriority = showErrorsByPriority;
        }

        public ModelValidation<T> WhenFalse<W>(string message,int priority, Func<W, bool> predicate, params Expression<Func<T, W>>[] property)
        {
            RunExpression(message,priority, c => !predicate(c(_model)), property);
            return this;
        }
       
        public ModelValidation<T> WhenTrue<W>(string message,int priority, Func<W, bool> predicate, params Expression<Func<T, W>>[] property)
        {
            RunExpression(message,priority, c => predicate(c(_model)), property);
            return this;
        }

        public ModelValidation<T> WhenNull(string message,int priority,  params Expression<Func<T, object>>[] property)
        {
            RunExpression(message,priority, (c) => c(_model) == null, property);
            return this;
        }

        public ModelValidation<T> WhenNull<W>(string message,int priority, params Expression<Func<T, Nullable<W>>>[] property) where W : struct
        {
            RunExpression(message,priority, c => !c(_model).HasValue, property);
            return this;
        }

        public ModelValidation<T> WhenNullOrEmpty(string message,int priority, params Expression<Func<T, string>>[] property)
        {
            RunExpression(message,priority, (c) => String.IsNullOrEmpty(c(_model)), property);
            return this;
        }

        public ModelValidation<T> WhenNullOrEmpty<W>(string message,int priority, params Expression<Func<T, IEnumerable<W>>>[] property)
        {
            RunExpression(message,priority, (c) => c(_model) == null || c(_model).Count() == 0, property);
            return this;
        }

        public ModelValidation<T> WhenNullOrEmpty(string message,int priority, params Expression<Func<T, IEnumerable>>[] property)
        {
            Func<IEnumerable, bool> hasAtLeastOneItem = new Func<IEnumerable,bool>((x) => {
                var res = x.GetEnumerator().MoveNext(); //TODO: Better way?
                return res;
            });

            RunExpression(message,priority, (c) => c(_model) == null || !hasAtLeastOneItem(c(_model)), property);
            return this;
        }
        public ModelValidation<T> InvalidEmailAddress(string message,int priority, params Expression<Func<T, string>>[] property)
        {
            var regex =@"^(?("")("".+?(?<!\\)""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
                @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-\w]*[0-9a-z]*\.)+[a-z0-9][\-a-z0-9]{0,22}[a-z0-9]))$";

            RunExpression(message,priority,(c)=> CheckRegularExpression(regex,c(_model)), property);
            return this;
        }
        public ModelValidation<T> NotSame(string message,int priority,string compareValue, params Expression<Func<T, string>>[] property)
        {
            var regex = @"^(?("")("".+?(?<!\\)""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
                @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-\w]*[0-9a-z]*\.)+[a-z0-9][\-a-z0-9]{0,22}[a-z0-9]))$";

            RunExpression(message,priority, (c) =>c(_model)!=compareValue, property);
            return this;
        }
        public ModelValidation<T> IsWeakPassword(string message,int priority,params Expression<Func<T, string>>[] property)
        {
            RunExpression(message,priority, (c) => ((int)PasswordStrength(c(_model)))<=3, property);
            return this;
        }
        public PasswordScore PasswordStrength(string password)
        {
            if (String.IsNullOrEmpty(password))
                return PasswordScore.Blank;
            int score = 0;
            var specialCharacters = new HashSet<char> { '/', '.', '[', '!', ',', '@', '#', '$', '%', '^', '&', '*', '?', '_', '~', '-', '£', '(', ')', ']', '/' };
            if (password.Any(char.IsLower))
                score++;
            if (password.Any(char.IsUpper))
                score++;
            if (password.Any(char.IsDigit))
                score++;
            if (password.Any(specialCharacters.Contains))
                score++;

            score = password.Length < 6 ? 3 : ++score;
           
           
               
            return (PasswordScore) score;
        }
        private bool CheckRegularExpression(string expression, string value )
        {
            try
            {
                var response = Regex.IsMatch(value,expression,
                      RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));
                return !response;
            }
            catch (RegexMatchTimeoutException)
            {
                return true;
            }
        }

        

        private void RunExpression<T, U>(string message,int priority, Func<Func<T, U>, bool> predicate, params Expression<Func<T, U>>[] property)
        {
            foreach (var exp in property)
            {
                _expressionCount++;
                var f = exp.Compile();

                if (predicate(f))
                {
                    string propName = GetPropertyNameFromExpression(exp);
                    _errors.Add(new ComponentModelValidationError(propName, string.Format(message, propName),priority));
                }
            }
        }

        private string GetPropertyNameFromExpression<T>(Expression<T> exp)
        {
            return (((MemberExpression)exp.Body).Member as PropertyInfo).Name;
        }

        public void ThrowWhenAllConditionsAreMet()
        {
            if (_errors.Count == _expressionCount)
            {
                throw new ComponentModelValidationException(_errors);
            }
        }

        public void ThrowWhenNoConditionsAreMet()
        {
            if (_errors.Count == 0)
            {
                throw new ComponentModelValidationException(_errors);
            }
        }

        public void ThrowWhenOneOrMoreConditionsAreMet()
        {
            ThrowIfErrors();
        }

        public void ThrowIfErrors()
        {
            if (_errors.Count > 0)
            {
                throw new ComponentModelValidationException(_errors);
            }
        }

        public bool IsValid {
            get { return _errors.Count == 0; }
        }

        public List<string> ErrorList
        {
            get
            {
                if (_errorsByPriority)
                {
                    var maxPriority = _errors.Min(p => p.Priority);
                    var errorslist = _errors.Where(p => p.Priority == maxPriority).Select(q => q.ErrorMessage).ToList();
                    return errorslist;
                }
                return _errors.Select(q => q.ErrorMessage).ToList();
            }
        }

        public static ModelValidation<T> Validate<T>(T model,bool errorbyPriority)
        {

            return new ModelValidation<T>(model,errorbyPriority);
        }
        public static ModelValidation<T> Validate<T>(T model)
        {

            return new ModelValidation<T>(model);
        }
    }
    public enum PasswordScore
    {
        Blank = 0,
        VeryWeak = 1,
        Weak = 2,
        Medium = 3,
        Strong = 4,
        VeryStrong = 5
    }

}
