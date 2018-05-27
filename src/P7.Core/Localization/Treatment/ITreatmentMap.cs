namespace P7.Core.Localization.Treatment
{
    public interface ITreatmentMap
    {
        ILocalizedStringResultTreatment GetTreatment(string key);
    }
}