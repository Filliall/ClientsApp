using System.Text.RegularExpressions;

namespace ClientsApp.Utils.Behaviors
{
    /// <summary>
    /// Um Behavior que restringe a entrada em um Entry para caracteres alfanuméricos,
    /// letras acentuadas e um conjunto de caracteres especiais comuns.
    /// Impede a entrada de outros símbolos, como emojis.
    /// </summary>
    public partial class AlphanumericValidationBehavior : Behavior<Entry>
    {
        // Esta Regex permite letras (incluindo acentos via \p{L}), números, espaços,
        // e os caracteres especiais: . , ' - / º ª
        [GeneratedRegex(@"^[a-zA-Z0-9\p{L}\s.,'-/ºª@#$%&*!~^]*$")]
        private static partial Regex AlphanumericRegex();

        protected override void OnAttachedTo(Entry entry)
        {
            base.OnAttachedTo(entry);
            entry.TextChanged += OnEntryTextChanged;
        }

        protected override void OnDetachingFrom(Entry entry)
        {
            base.OnDetachingFrom(entry);
            entry.TextChanged -= OnEntryTextChanged;
        }

        private void OnEntryTextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(e.NewTextValue))
                return;

            if (!AlphanumericRegex().IsMatch(e.NewTextValue))
            {
                ((Entry)sender).Text = e.OldTextValue;
            }
        }
    }
}