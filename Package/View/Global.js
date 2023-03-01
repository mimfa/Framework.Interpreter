function print(message, inline = false) {
	console.Write(Text(message), inline);
}
function echo(message, inline = false) {
	console.Write(message + "", inline);
}	
function alert(message, trsl = false) {
	return Library.MiMFa.Service.DialogService.ShowMessage(MessageMode.Message, trsl, message + "") == Library.System.Windows.Forms.DialogResult.OK;
}
function confirm(message, trsl = false) {
	return Library.MiMFa.Service.DialogService.ShowMessage(MessageMode.Question, trsl, message + "") == Library.System.Windows.Forms.DialogResult.Yes;
}
function prompt(message, defaultValue = "", trsl = false) {
	return Library.MiMFa.Service.DialogService.GetMessage(message+ "", defaultValue+ "", trsl);
}
function warning(message, trsl = false) {
	const res = Library.MiMFa.Service.DialogService.ShowMessage(MessageMode.Warning, trsl, message + "");
	return res == Library.System.Windows.Forms.DialogResult.Yes? true :res == Library.System.Windows.Forms.DialogResult.No? false : null;
}


"Global View Library Installed Successfully"