//CONVERT TO STRING FUNCTIONS
const Text = function (obj, nest = 0)
{
	if(obj === undefined || obj === null)
		return obj + "";
	
	let objt = typeof obj;
	if(objt === 'string')
		return obj;
	
	let indent = n => n >= 0? (n > 0? "\r\n" : "") + loop(0,n, i => "\t").join("") : "";
	let ind = indent(nest);
	let indp = indent(nest+1);
	if(Array.isArray(obj))
		return ind + 
			"[" +
			each(obj, (x,i) => Text(x, nest + 1)).join(", ") +
			"]";
	try{
		if(objt === 'object' && isArray(obj))
		{
			let cobj = new Array();
			for(let x of obj)
				cobj.push(Text(x, nest + 1));
			return ind + "[" + cobj.join(", ") + "]";
		}
		if(objt === 'object')
		{
			// let cobj = new Array();
			// for(let x in obj)
				// cobj.push(x + ": " + Text(obj[x], nest + 1).trim());
			// return ind + "{" + indp + cobj.join(", " + indp) + ind + "}";
			return JSON.stringify(obj);
		}
	}
	catch{return obj + "";}
	
	return obj + "";
}

//GLOBAL Text OBJECTS
Text.toHtml = (obj) => htmlConvertor.Done(obj);
Text.tryToHtml = (obj) => htmlConvertor.TryDone(obj);
Text.translate = (keys) => Library.MiMFa.Default.Translator.Get(keys);
Text.get = (obj) => Library.MiMFa.Default.Translator.Get(Text(obj));
Text.set = (key, val) => Library.MiMFa.Default.Translator.Set(key, Text(val));

// Expose Text and __ identifiers
__ = Text;

"Text Library Installed Successfully"