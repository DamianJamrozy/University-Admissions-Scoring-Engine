using Microsoft.AspNetCore.Mvc;
using University_Admissions_Scoring_Engine.Areas.Admin.ViewModels;

namespace University_Admissions_Scoring_Engine.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class DashboardController : Controller
    {
        public IActionResult Index()
        {
            var vm = new AdminDashboardViewModel
            {
                Title = "Panel administratora",
                Subtitle = "Centralne miejsce zarządzania systemem rekrutacji.",
                Sections = new List<AdminDashboardSectionViewModel>
                {
                    new AdminDashboardSectionViewModel
                    {
                        Title = "Aktualne moduły",
                        Description = "Szybki dostęp do gotowych lub rozwijanych obszarów systemu.",
                        Cards = new List<AdminDashboardCardViewModel>
                        {
                            new AdminDashboardCardViewModel
                            {
                                Title = "Algorytmy",
                                Description = "Zarządzanie algorytmami punktacji, strukturą grup i testowaniem logiki.",
                                Url = "/Admin/Algorytmy",
                                Badge = "Aktywne",
                                Icon = "bi-diagram-3",
                                IsEnabled = true
                            },
                            new AdminDashboardCardViewModel
                            {
                                Title = "Kierunki",
                                Description = "Pełny CRUD kierunków, limity, przypisanie algorytmów i parametry naboru.",
                                Url = "/Admin/Kierunki",
                                Badge = "Aktywne",
                                Icon = "bi-mortarboard",
                                IsEnabled = true
                            },
                            new AdminDashboardCardViewModel
                            {
                                Title = "Matury",
                                Description = "Zarządzanie typami matur, skalami punktowymi i ich konfiguracją.",
                                Url = "/Admin/Matury",
                                Badge = "Aktywne",
                                Icon = "bi-journal-check",
                                IsEnabled = true
                            },
                            new AdminDashboardCardViewModel
                            {
                                Title = "Przedmioty",
                                Description = "Obsługa przedmiotów, wariantów oraz przypisań do matur.",
                                Url = "/Admin/Przedmioty",
                                Badge = "Aktywne",
                                Icon = "bi-book",
                                IsEnabled = true
                            },
                            new AdminDashboardCardViewModel
                            {
                                Title = "Rankingi",
                                Description = "Przeliczanie punktów, generowanie rankingów i statusów kandydatów.",
                                Url = "/Admin/Rankingi",
                                Badge = "Aktywne",
                                Icon = "bi-bar-chart-line",
                                IsEnabled = true
                            }
                        }
                    },
                    new AdminDashboardSectionViewModel
                    {
                        Title = "Moduły do rozwoju",
                        Description = "Obszary wynikające bezpośrednio z docelowej wizji systemu.",
                        Cards = new List<AdminDashboardCardViewModel>
                        {
                            new AdminDashboardCardViewModel
                            {
                                Title = "CMS treści",
                                Description = "Drzewiasta struktura stron, widoczność, wersje językowe, artykuły, sekcje i multimedia.",
                                Url = "#",
                                Badge = "Planowany",
                                Icon = "bi-folder2-open",
                                IsEnabled = false
                            },
                            new AdminDashboardCardViewModel
                            {
                                Title = "Formularze kandydatów",
                                Description = "Dynamiczne formularze rejestracji, danych osobowych i zapisów na kierunki.",
                                Url = "#",
                                Badge = "Planowany",
                                Icon = "bi-ui-checks-grid",
                                IsEnabled = false
                            },
                            new AdminDashboardCardViewModel
                            {
                                Title = "Języki systemu",
                                Description = "Aktywacja wersji językowych i zarządzanie dostępnymi tłumaczeniami.",
                                Url = "#",
                                Badge = "Planowany",
                                Icon = "bi-translate",
                                IsEnabled = false
                            },
                            new AdminDashboardCardViewModel
                            {
                                Title = "Informator kierunków",
                                Description = "Edycja treści informatora, sekcji, multimediów i danych powiązanych z kierunkiem.",
                                Url = "#",
                                Badge = "Planowany",
                                Icon = "bi-file-earmark-richtext",
                                IsEnabled = false
                            },
                            new AdminDashboardCardViewModel
                            {
                                Title = "Akceptacja zdjęć",
                                Description = "Weryfikacja zdjęć do legitymacji, odrzucenie, akceptacja i komunikaty do kandydata.",
                                Url = "#",
                                Badge = "Planowany",
                                Icon = "bi-person-bounding-box",
                                IsEnabled = false
                            }
                        }
                    },
                    new AdminDashboardSectionViewModel
                    {
                        Title = "Bezpieczeństwo i jakość",
                        Description = "Założenia architektoniczne prowadzące dalszy rozwój panelu.",
                        Cards = new List<AdminDashboardCardViewModel>
                        {
                            new AdminDashboardCardViewModel
                            {
                                Title = "Bezpieczne formularze",
                                Description = "Wszystkie formularze administracyjne projektujemy pod walidację, ograniczenia typów i kontrolę dostępu.",
                                Url = "#",
                                Badge = "Założenie",
                                Icon = "bi-shield-check",
                                IsEnabled = false
                            },
                            new AdminDashboardCardViewModel
                            {
                                Title = "Izolacja panelu admina",
                                Description = "Panel administratora rozwijamy jako wydzieloną warstwę interfejsu i logiki zarządczej.",
                                Url = "#",
                                Badge = "Założenie",
                                Icon = "bi-lock",
                                IsEnabled = false
                            },
                            new AdminDashboardCardViewModel
                            {
                                Title = "Nowoczesny UI/UX",
                                Description = "Spójny, klasyczny, nowoczesny interfejs z naciskiem na czytelność i szybkość obsługi.",
                                Url = "#",
                                Badge = "Założenie",
                                Icon = "bi-layout-text-window",
                                IsEnabled = false
                            }
                        }
                    }
                }
            };

            return View(vm);
        }
    }
}