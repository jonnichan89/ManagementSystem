namespace ManagementSystem.Common.Models
{
    public class ServiceActionResult
    {
        public List<ModelValidation> ModelValidations { get; set; }
        public bool ActionIsSuccess { get; set; }
        public object? ActionResult { get; set; }
        public string ErrorMessage { get; set; }
    }

    public class ModelValidation
    {
        public string ObjectTypeName { get; set; }
        public string PropertyName { get; set; }
        public ModelValidationType ErrorType { get; set; }
        public string ErrorMessage { get; set; }
        public string PropertyValue { get; set; }

        public ModelValidation() { }

        public ModelValidation(string objectTypeName, string propertyName, string propertyValue, ModelValidationType errorType)
            : this(objectTypeName, propertyName, propertyValue, errorType, "") { }

        public ModelValidation(string objectTypeName, string propertyName, string propertyValue, ModelValidationType errorType, string errorMessage)
        {
            ObjectTypeName = objectTypeName;
            PropertyName = propertyName;
            PropertyValue = propertyValue;
            ErrorType = errorType;
            ErrorMessage = errorMessage;
        }

        public static ModelValidation FieldIsRequired(string objectTypeName, string propertyName, string propertyValue)
        {
            return new ModelValidation(objectTypeName, propertyName, propertyValue, ModelValidationType.FieldIsRequired, "");
        }

        public static ModelValidation DuplicateRecord(string objectTypeName, string propertyName, string propertyValue)
        {
            return new ModelValidation(objectTypeName, propertyName, propertyValue, ModelValidationType.DuplicateRecord, "");
        }

        public static ModelValidation PKAlreadyExists(string objectTypeName, string propertyName, string propertyValue)
        {
            return new ModelValidation(objectTypeName, propertyName, propertyValue, ModelValidationType.PKAlreadyExists, "");
        }
    }

    public enum ModelValidationType
    {
        None = 0,
        FieldIsRequired = 1,
        PKAlreadyExists = 2,
        DuplicateRecord = 3
    }
}
