using Microsoft.AspNetCore.Components.Forms;

namespace GeoNRage.App.Core;

public static class EditContextExtensions
{
    public static void UpdateCssClassProvider(this EditContext? context)
    {
        context?.SetFieldCssClassProvider(new CustomFieldClassProvider());
    }

    private sealed class CustomFieldClassProvider : FieldCssClassProvider
    {
        public override string GetFieldCssClass(EditContext editContext, in FieldIdentifier fieldIdentifier)
        {
            bool isValid = !editContext.GetValidationMessages(fieldIdentifier).Any();
            return isValid ? "" : "is-danger";
        }
    }
}
