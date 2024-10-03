namespace LoginSystem.Api.Constants;

/// <summary>
///     Global storage of error codes which will be returned by the System API
/// </summary>
public static class ErrorCodes
{
    public const string UserNumberInUse = "USER_NUMBER_IN_USE";

    public const string EmailAddressInUse = "EMAIL_ADDRESS_IN_USE";

    public const string MobileNumberInUse = "MOBILE_NUMBER_IN_USE";

    public const string UserNameInUse = "USER_NAME_IN_USE";

    public const string InvalidStatus = "INVALID_STATUS";

    public const string NoCardProgramme = "NO_CARD_PROGRAMME";

    public const string NoSuchOrganisationError = "NO_SUCH_ORGANISATION";

    public const string NotFoundError = "NOT_FOUND";

    public const string ReadOnlyFieldUpdate = "READ_ONLY_FIELD_UPDATE";

    public const string ReferenceAlreadyUsed = "REFERENCE_ALREADY_USED";

    public const string SubscriptionNameInUse = "SUBSCRIPTION_NAME_IN_USE";

    public const string ValidationError = "VALIDATION_ERROR";
}

/// <summary>
///     Global storage of error messages which will be returned by the System API
/// </summary>
public static class ErrorMessages
{
    public const string NotFoundCustomerNumber = "No customer was found with the provided customer number.";

    public const string NotFoundOrganisation = "No organisation was found with the provided subscription name.";
}
