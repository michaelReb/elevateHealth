namespace NMSE.ViewModels;

public class SimulationModelViewModel : ViewModel {



  bool _IsSelected;
  public bool IsSelected { get => _IsSelected; set => SetField(ref _IsSelected, value); }


  string _Name;
  public string Name { get => _Name; set => SetField(ref _Name, value); }

  string _DisplayText;
  public string DisplayText { get => _DisplayText; set => SetField(ref _DisplayText, value); }

  string _Description;
  public string Description { get => _Description; set => SetField(ref _Description, value); }



  string _ModelTransitionsImage;
  public string ModelTransitionsImage { get => _ModelTransitionsImage; set => SetField(ref _ModelTransitionsImage, value); }


  IList<string> _Urls = [];
  public IList<string> Urls { get => _Urls; set => SetField(ref _Urls, value); }

  public static IReadOnlyList<SimulationModelViewModel> AvailableModels = [
    new() {
      Name = "Base",
      DisplayText = "Base Model (published version)",
      Description = $@"This Model is a predictive model describing the patient funnel until symptom-free disease status as ultimative goal for medical therapy. The model allows simulations and probability calculations of different campaigns (interventions) to improve the patient journey. It’s a generalized model based on the main characteristics of patients (being symptomatic, having received a diagnosed, getting treatment, adaption of treatment) and based on real-world data and/ or expert knowledge. 
• Each patient will reach a new disease condition by a certain action performed by the treating physician (making a diagnosis, prescribing treatment, go for treatment escalation) - displayed by the arrow.
• The grey box refers to patients under physician-care.
• White background refers to self-medication/ no treatment.
• This Markov Model based on 7 disease conditions (= states) of the patient (displayed by the oval).
• The patient flow is marked by the transition (diagnosis, treatment, treatment escalation) with a distinct probability based on indication- and country-specific data (diagnosis rate, treatment rate, treatment escalation rate).
• Underlying data must be adapted according to the disease-specific and country-specific values. Please, see menu.
Keep in mind: this is a simplified model allowing minimal data input and general overview. 
The model is powerful to provide a holistic view on the main pain points for internal and external applications.
CAVE: Disease specific remission is implemented in the model (might change in other diseases than chronic spontaneous urticaria).
Please refer: ""Modelling of patient journey in chronic spontaneous urticaria: Increasing awareness and education by shorten patients' disease journey in Germany"", Maurer et al (doi: 10.1111/jdv.19940. Epub 2024 Mar 5.)",
      ModelTransitionsImage = "pack://application:,,,/NMSE;component/Images/transitions-base-model.png",
      Urls = []
    },
        new() {
      Name = "BaseImproved",
      DisplayText = "Base Model (improved)",
      Description = $@"The improved base model is similar to the base model, except for a single change in the treatment escalation pattern. In the improved model, it is assumed that the likelihood of physicians prescribing biologics is relatively low. Therefore, it incorporates a 5% probability that physicians will escalate treatment to the third option for an uncontrolled patient (instead of 100% treatment escalation in the base model).",
      ModelTransitionsImage = "pack://application:,,,/NMSE;component/Images/transitions-base-model.png",
      Urls = []
    },
    //new() {
    //  Name = "Bridge",
    //  DisplayText = "Bridge Model",
    //  Description = $@"The Bridge model is based on the improved model, but with the added feature of being able to modify three interventions to enhance patient care. This model assumes that by providing specific disease education to physicians, there can be improvements in diagnosis, treatment initiation, and treatment escalation in a specific manner.",
    //  ModelTransitionsImage = "pack://application:,,,/NMSE;component/Images/transitions-base-model.png",
    //  Urls = []
    //},
    //new() {
    //  Name = "DigiMoc",
    //  DisplayText = "Digi Moc Model",
    //  Description = $@"The DigiMoc model builds upon the improved model, offering the additional capability to customize three interventions to enhance patient care. This model specifically incorporates digital tools aimed at improving patient therapy management.",
    //  ModelTransitionsImage = "pack://application:,,,/NMSE;component/Images/transitions-base-model.png",
    //  Urls = []
    //}
  ];
}
