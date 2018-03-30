using System;

namespace MzIdentML
{
    public partial interface IMzIdentML : IIdentifiableType
    {
        ICvType[] cvList { get; set; }
        IAnalysisSoftwareType[] AnalysisSoftwareList { get; set; }
        IProviderType Provider { get; set; }
        IAbstractContactType[] AuditCollection { get; set; }
        ISampleType[] AnalysisSampleCollection { get; set; }
        ISequenceCollectionType SequenceCollection { get; set; }
        IAnalysisCollectionType AnalysisCollection { get; set; }
        IAnalysisProtocolCollectionType AnalysisProtocolCollection { get; set; }
        IDataCollectionType DataCollection { get; set; }
        IBibliographicReferenceType[] BibliographicReference { get; set; }
        DateTime creationDate { get; set; }
        bool creationDateSpecified { get; set; }
        string version { get; set; }
    }

    public partial interface ICvType
    {
        string fullName { get; set; }
        string version { get; set; }
        string uri { get; set; }
        string id { get; set; }
    }

    public interface ISpectrumIdentificationItemRefType
    {
        string spectrumIdentificationItem_ref { get; set; }
    }

    public interface IPeptideHypothesisType
    {
        ISpectrumIdentificationItemRefType[] SpectrumIdentificationItemRef { get; set; }
        string peptideEvidence_ref { get; set; }
    }

    public interface IFragmentArrayType
    {
        float[] values { get; set; }
        string measure_ref { get; set; }
    }

    public interface IIonTypeType
    {
        IFragmentArrayType[] FragmentArray { get; set; }
        ICVParamType cvParam { get; set; }
        string[] index { get; set; }
        int charge { get; set; }
    }

    public interface ICVParamType : IAbstractParamType
    {
        string cvRef { get; set; }
        string accession { get; set; }
    }

    public partial interface IAbstractParamType
    {
        string name { get; set; }
        string value { get; set; }
        string unitAccession { get; set; }
        string unitName { get; set; }
        string unitCvRef { get; set; }
    }

    public interface IUserParamType : IAbstractParamType
    {
        string type { get; set; }
    }

    public interface IPeptideEvidenceRefType
    {
        string peptideEvidence_ref { get; set; }
    }

    public interface IAnalysisDataType
    {
        ISpectrumIdentificationListType[] SpectrumIdentificationList { get; set; }
        IProteinDetectionListType ProteinDetectionList { get; set; }
    }

    public interface ISpectrumIdentificationListType : IIdentifiableType
    {
        string id { get; set; }
        string name { get; set; }
        IMeasureType[] FragmentationTable { get; set; }
        ISpectrumIdentificationResultType[] SpectrumIdentificationResult { get; set; }
        ICVParamType[] cvParam { get; set; }
        IUserParamType[] userParam { get; set; }
        long numSequencesSearched { get; set; }
        bool numSequencesSearchedSpecified { get; set; }
    }

    public interface IMeasureType : IIdentifiableType
    {
        string id { get; set; }
        string name { get; set; }
        ICVParamType[] cvParam { get; set; }
    }

    public partial interface IIdentifiableType
    {
        string id { get; set; }
        string name { get; set; }
    }

    public partial interface IBibliographicReferenceType : IIdentifiableType
    {
        string authors { get; set; }
        string publication { get; set; }
        string publisher { get; set; }
        string editor { get; set; }
        int year { get; set; }
        bool yearSpecified { get; set; }
        string volume { get; set; }
        string issue { get; set; }
        string pages { get; set; }
        string title { get; set; }
        string doi { get; set; }
    }

    public interface IProteinDetectionHypothesisType : IIdentifiableType
    {
        string id { get; set; }
        string name { get; set; }
        IPeptideHypothesisType[] PeptideHypothesis { get; set; }
        ICVParamType[] cvParam { get; set; }
        IUserParamType[] userParam { get; set; }
        string dBSequence_ref { get; set; }
        bool passThreshold { get; set; }
    }

    public interface IProteinAmbiguityGroupType : IIdentifiableType
    {
        string id { get; set; }
        string name { get; set; }
        IProteinDetectionHypothesisType[] ProteinDetectionHypothesis { get; set; }
        ICVParamType[] cvParam { get; set; }
        IUserParamType[] userParam { get; set; }
    }

    public interface IProteinDetectionListType : IIdentifiableType
    {
        string id { get; set; }
        string name { get; set; }
        IProteinAmbiguityGroupType[] ProteinAmbiguityGroup { get; set; }
        ICVParamType[] cvParam { get; set; }
        IUserParamType[] userParam { get; set; }
    }

    public interface ISpectrumIdentificationItemType : IIdentifiableType
    {
        string id { get; set; }
        string name { get; set; }
        IPeptideEvidenceRefType[] PeptideEvidenceRef { get; set; }
        IIonTypeType[] Fragmentation { get; set; }
        ICVParamType[] cvParam { get; set; }
        IUserParamType[] userParam { get; set; }
        int chargeState { get; set; }
        double experimentalMassToCharge { get; set; }
        double calculatedMassToCharge { get; set; }
        bool calculatedMassToChargeSpecified { get; set; }
        float calculatedPI { get; set; }
        bool calculatedPISpecified { get; set; }
        string peptide_ref { get; set; }
        int rank { get; set; }
        bool passThreshold { get; set; }
        string massTable_ref { get; set; }
        string sample_ref { get; set; }
    }

    public interface ISpectrumIdentificationResultType : IIdentifiableType
    {
        string id { get; set; }
        string name { get; set; }
        ISpectrumIdentificationItemType[] SpectrumIdentificationItem { get; set; }
        ICVParamType[] cvParam { get; set; }
        IUserParamType[] userParam { get; set; }
        string spectrumID { get; set; }
        string spectraData_ref { get; set; }
    }

    public interface IExternalDataType : IIdentifiableType
    {
        string id { get; set; }
        string name { get; set; }
        string ExternalFormatDocumentation { get; set; }
        IFileFormatType FileFormat { get; set; }
        string location { get; set; }
    }

    public interface IFileFormatType
    {
        ICVParamType cvParam { get; set; }
    }

    public interface ISpectraDataType : IExternalDataType
    {
        ISpectrumIDFormatType SpectrumIDFormat { get; set; }
    }

    public interface ISpectrumIDFormatType
    {
        ICVParamType cvParam { get; set; }
    }

    public interface ISourceFileType : IExternalDataType
    {
        ICVParamType[] cvParam { get; set; }
        IUserParamType[] userParam { get; set; }
    }

    public interface ISearchDatabaseType : IExternalDataType
    {
        IParamType DatabaseName { get; set; }
        ICVParamType[] cvParam { get; set; }
        string version { get; set; }
        DateTime releaseDate { get; set; }
        bool releaseDateSpecified { get; set; }
        long numDatabaseSequences { get; set; }
        bool numDatabaseSequencesSpecified { get; set; }
        long numResidues { get; set; }
        bool numResiduesSpecified { get; set; }
    }

    public partial interface IParamType
    {
        IAbstractParamType Item { get; set; }
    }

    public interface IProteinDetectionProtocolType : IIdentifiableType
    {
        string id { get; set; }
        string name { get; set; }
        IParamListType AnalysisParams { get; set; }
        IParamListType Threshold { get; set; }
        string analysisSoftware_ref { get; set; }
    }

    public interface IParamListType
    {
        IAbstractParamType[] Items { get; set; }
    }

    public interface ITranslationTableType : IIdentifiableType
    {
        string id { get; set; }
        string name { get; set; }
        ICVParamType[] cvParam { get; set; }
    }

    public interface IMassTableType : IIdentifiableType
    {
        string id { get; set; }
        string name { get; set; }
        IResidueType[] Residue { get; set; }
        IAmbiguousResidueType[] AmbiguousResidue { get; set; }
        ICVParamType[] cvParam { get; set; }
        IUserParamType[] userParam { get; set; }
        string[] msLevel { get; set; }
    }

    public interface IResidueType
    {
        string code { get; set; }
        float mass { get; set; }
    }

    public interface IAmbiguousResidueType
    {
        ICVParamType[] cvParam { get; set; }
        IUserParamType[] userParam { get; set; }
        string code { get; set; }
    }

    public interface IEnzymeType : IIdentifiableType
    {
        string id { get; set; }
        string name { get; set; }
        string SiteRegexp { get; set; }
        IParamListType EnzymeName { get; set; }
        string nTermGain { get; set; }
        string cTermGain { get; set; }
        bool semiSpecific { get; set; }
        bool semiSpecificSpecified { get; set; }
        int missedCleavages { get; set; }
        bool missedCleavagesSpecified { get; set; }
        int minDistance { get; set; }
        bool minDistanceSpecified { get; set; }
    }

    public interface ISpectrumIdentificationProtocolType : IIdentifiableType
    {
        string id { get; set; }
        string name { get; set; }
        IParamType SearchType { get; set; }
        IParamListType AdditionalSearchParams { get; set; }
        ISearchModificationType[] ModificationParams { get; set; }
        IEnzymesType Enzymes { get; set; }
        IMassTableType[] MassTable { get; set; }
        ICVParamType[] FragmentTolerance { get; set; }
        ICVParamType[] ParentTolerance { get; set; }
        IParamListType Threshold { get; set; }
        IFilterType[] DatabaseFilters { get; set; }
        IDatabaseTranslationType DatabaseTranslation { get; set; }
        string analysisSoftware_ref { get; set; }
    }

    public interface ISearchModificationType
    {
        ICVParamType[] SpecificityRules { get; set; }
        ICVParamType[] cvParam { get; set; }
        bool fixedMod { get; set; }
        float massDelta { get; set; }
        string residues { get; set; }
    }

    public interface IEnzymesType
    {
        IEnzymeType[] Enzyme { get; set; }
        bool independent { get; set; }
        bool independentSpecified { get; set; }
    }

    public interface IFilterType
    {
        IParamType FilterType1 { get; set; }
        IParamListType Include { get; set; }
        IParamListType Exclude { get; set; }
    }

    public interface IDatabaseTranslationType
    {
        ITranslationTableType[] TranslationTable { get; set; }
        int[] frames { get; set; }
    }

    public interface IProtocolApplicationType : IIdentifiableType
    {
        string id { get; set; }
        string name { get; set; }
        DateTime activityDate { get; set; }
        bool activityDateSpecified { get; set; }
    }

    public interface IProteinDetectionType : IProtocolApplicationType
    {
        IInputSpectrumIdentificationsType[] InputSpectrumIdentifications { get; set; }
        string proteinDetectionList_ref { get; set; }
        string proteinDetectionProtocol_ref { get; set; }
    }

    public interface IInputSpectrumIdentificationsType
    {
        string spectrumIdentificationList_ref { get; set; }
    }

    public interface ISpectrumIdentificationType : IProtocolApplicationType
    {
        IInputSpectraType[] InputSpectra { get; set; }
        ISearchDatabaseRefType[] SearchDatabaseRef { get; set; }
        string spectrumIdentificationProtocol_ref { get; set; }
        string spectrumIdentificationList_ref { get; set; }
    }

    public interface IInputSpectraType
    {
        string spectraData_ref { get; set; }
    }

    public interface ISearchDatabaseRefType
    {
        string searchDatabase_ref { get; set; }
    }

    public interface IPeptideEvidenceType : IIdentifiableType
    {
        ICVParamType[] cvParam { get; set; }
        IUserParamType[] userParam { get; set; }
        string dBSequence_ref { get; set; }
        string peptide_ref { get; set; }
        int start { get; set; }
        bool startSpecified { get; set; }
        int end { get; set; }
        bool endSpecified { get; set; }
        string pre { get; set; }
        string post { get; set; }
        string translationTable_ref { get; set; }
        int frame { get; set; }
        bool frameSpecified { get; set; }
        bool isDecoy { get; set; }
    }

    public interface IPeptideType : IIdentifiableType
    {
        string PeptideSequence { get; set; }
        IModificationType[] Modification { get; set; }
        ISubstitutionModificationType[] SubstitutionModification { get; set; }
        ICVParamType[] cvParam { get; set; }
        IUserParamType[] userParam { get; set; }
    }

    public interface IModificationType
    {
        ICVParamType[] cvParam { get; set; }
        int location { get; set; }
        bool locationSpecified { get; set; }
        string[] residues { get; set; }
        double avgMassDelta { get; set; }
        bool avgMassDeltaSpecified { get; set; }
        double monoisotopicMassDelta { get; set; }
        bool monoisotopicMassDeltaSpecified { get; set; }
    }

    public interface ISubstitutionModificationType
    {
        string originalResidue { get; set; }
        string replacementResidue { get; set; }
        int location { get; set; }
        bool locationSpecified { get; set; }
        double avgMassDelta { get; set; }
        bool avgMassDeltaSpecified { get; set; }
        double monoisotopicMassDelta { get; set; }
        bool monoisotopicMassDeltaSpecified { get; set; }
    }

    public interface IDBSequenceType : IIdentifiableType
    {
        string Seq { get; set; }
        ICVParamType[] cvParam { get; set; }
        IUserParamType[] userParam { get; set; }
        int length { get; set; }
        bool lengthSpecified { get; set; }
        string searchDatabase_ref { get; set; }
        string accession { get; set; }
    }

    public partial interface ISampleType : IIdentifiableType
    {
        IContactRoleType[] ContactRole { get; set; }
        ISubSampleType[] SubSample { get; set; }
        ICVParamType[] cvParam { get; set; }
        IUserParamType[] userParam { get; set; }
    }

    public partial interface IContactRoleType
    {
        IRoleType Role { get; set; }
        string contact_ref { get; set; }
    }

    public interface IRoleType
    {
        ICVParamType cvParam { get; set; }
    }

    public interface ISubSampleType
    {
        string sample_ref { get; set; }
    }

    public partial interface IAbstractContactType : IIdentifiableType
    {
        ICVParamType[] cvParam { get; set; }
        IUserParamType[] userParam { get; set; }
    }

    public partial interface IOrganizationType : IAbstractContactType
    {
        IParentOrganizationType Parent { get; set; }
    }

    public interface IParentOrganizationType
    {
        string organization_ref { get; set; }
    }

    public partial interface IPersonType : IAbstractContactType
    {
        IAffiliationType[] Affiliation { get; set; }
        string lastName { get; set; }
        string firstName { get; set; }
        string midInitials { get; set; }
    }

    public interface IAffiliationType
    {
        string organization_ref { get; set; }
    }

    public partial interface IProviderType : IIdentifiableType
    {
        IContactRoleType ContactRole { get; set; }
        string analysisSoftware_ref { get; set; }
    }

    public partial interface IAnalysisSoftwareType : IIdentifiableType
    {
        string id { get; set; }
        string name { get; set; }
        IContactRoleType ContactRole { get; set; }
        IParamType SoftwareName { get; set; }
        string Customizations { get; set; }
        string version { get; set; }
        string uri { get; set; }
    }

    public interface IInputsType
    {
        ISourceFileType[] SourceFile { get; set; }
        ISearchDatabaseType[] SearchDatabase { get; set; }
        ISpectraDataType[] SpectraData { get; set; }
    }

    public partial interface IDataCollectionType
    {
        IInputsType Inputs { get; set; }
        IAnalysisDataType AnalysisData { get; set; }
    }

    public partial interface IAnalysisProtocolCollectionType
    {
        ISpectrumIdentificationProtocolType[] SpectrumIdentificationProtocol { get; set; }
        IProteinDetectionProtocolType ProteinDetectionProtocol { get; set; }
    }

    public partial interface IAnalysisCollectionType
    {
        ISpectrumIdentificationType[] SpectrumIdentification { get; set; }
        IProteinDetectionType ProteinDetection { get; set; }
    }

    public partial interface ISequenceCollectionType
    {
        IDBSequenceType[] DBSequence { get; set; }
        IPeptideType[] Peptide { get; set; }
        IPeptideEvidenceType[] PeptideEvidence { get; set; }
    }
}