using ClientsApp.Services;

namespace ClientsApp.Behaviors
{
    public class NumericValidationBehavior : Behavior<Entry>
    {
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

        private async void OnEntryTextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(e.NewTextValue))
                return;

            if (!e.NewTextValue.ToCharArray().All(char.IsDigit))
            {
                ((Entry)sender).Text = e.OldTextValue;

                // Usando o ServiceProvider para obter o serviço de diálogo,
                // já que Behaviors não suportam injeção de dependência diretamente.
                // Isso requer configuração no MauiProgram.cs.
                var dialogService = IPlatformApplication.Current?.Services.GetService<IDialogService>();
                await (dialogService?.DisplayAlert("Entrada Inválida", "O campo Idade aceita apenas números.", "OK") ?? Task.CompletedTask);
            }
        }
    }
}
