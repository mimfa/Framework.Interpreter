//SYSTEM FUNCTIONS
const System = function ()
{
	return arguments;
}

//GLOBAL SYSTEM PROPERTIES
Object.defineProperty(this, "System.Environment", {
	get: function () { return Library.System.Environment; }
});
Object.defineProperty(this, "System.Translator", {
	get: function () { return Library.MiMFa.Default.Translator; },
	set: function (arg) { Library.MiMFa.Default.Translator = arg; }
});
Object.defineProperty(this, "System.Templator", {
	get: function () { return Library.MiMFa.Default.Templator; },
	set: function (arg) { Library.MiMFa.Default.Templator = arg; }
});
Object.defineProperty(this, "System.IsRightToLeft", {
	get: function () { return Library.MiMFa.Default.RightToLeft; },
	set: function (arg) { Library.MiMFa.Default.RightToLeft = arg; }
});
Object.defineProperty(this, "System.Language", {
	get: function () { return Library.MiMFa.Default.Translator.Language; },
	set: function (arg) { Library.MiMFa.Default.Translator.Language = arg; }
});
Object.defineProperty(this, "System.Lang", {
	get: function () { return Library.MiMFa.Default.Translator.Lang; },
	set: function (arg) { Library.MiMFa.Default.Translator.Lang = arg; }
});
Object.defineProperty(this, "System.CharSet", {
	get: function () { return Library.MiMFa.Default.Translator.CharSet; },
	set: function (arg) { Library.MiMFa.Default.Translator.CharSet = arg; }
});
Object.defineProperty(this, "System.HorizontalAlign", {
	get: function () { return Library.MiMFa.Default.RightToLeft ? "right" : "left"; }
});
Object.defineProperty(this, "System.VerticalAlign", {
	get: function () { return "top"; }
});

"System Library Installed Successfully"