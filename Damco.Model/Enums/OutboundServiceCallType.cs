namespace Damco.Model.Enums
{
    public enum OutboundServiceCallType
    {
        SendImmediately,
        SendViaQueue,
        SendViaBackgroundProcess,
        ResendImmediately,
        ResendViaQueue,
        ResendViaBackgroundProcess
    }
}
