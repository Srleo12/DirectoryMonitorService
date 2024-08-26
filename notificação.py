from win10toast import ToastNotifier

toaster = ToastNotifier()

toaster.show_toast(
    "Notificação",
    "Serviço Sistema de Monitoramento iniciado!",
    threaded=True,
    icon_path=None,
    duration=2
)