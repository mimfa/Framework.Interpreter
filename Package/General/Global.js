let INITTIME = new Date();
let ID = (new Date(
	INITTIME.getUTCFullYear()-51,
	INITTIME.getUTCMonth(),
	INITTIME.getUTCDay(),
	INITTIME.getUTCHours(),
	INITTIME.getUTCMinutes(),
	INITTIME.getUTCSeconds(),
	INITTIME.getUTCMilliseconds())).getTime();

//const MiMFa = Library.MiMFa;
const once = engine.Once;
const use = engine.Use;
const useAsync = engine.UseAsync;
const evaluate = engine.Evaluate;
const execute = engine.Execute;
const compile = engine.Compile;


const isNull         	= function(obj) {
	return obj === null;
}

const isNaN       	 	= function(obj) {
	return typeof obj != "string" && obj+"" === "NaN";
}

const isEmpty         	= function(obj) {
	return obj === null || obj === "" || obj === {} || obj === [];
}					


const isDefined        	= function(obj) {
	return !isUndefined(obj);
}

const isUndefined        	= function(obj) {
	return obj === undefined;
}

const isHollow        	= function(obj) {
	return obj === undefined || isNaN(obj) || isEmpty(obj) || (obj + "").trim() === "";
}

const isSet        	= function(obj) {
	return !isUnset(obj);
}

const isUnset        	= function(obj) {
	return obj === undefined || obj === null || isNaN(obj);
}

const isString       	= function(obj) {
	return typeof obj === 'string';
}

const isText	       	= function(obj) {
	return isString(obj) && !obj.trimStart().startsWith("<");
}

const isObject       	= function(obj) {
	return typeof obj === 'object';
}

const isNumber       	= function(obj) {
	return parseFloat(obj) + "" === obj + "";
}

const isInt				= function(n){
    return Number(n) === n && n % 1 === 0;
}

const isFloat			= function(n){
    return Number(n) === n && n % 1 !== 0;
}

const isArray        	= function(obj, dim = 1) {
	if(isHollow(obj)) return false;
	let objt = typeof obj;
	if(objt === 'string') return false;
	if(Array.isArray(obj)) return --dim == 0 || isArray(obj[0],dim);
	if(objt === 'object' && (obj["Count"] !== undefined || obj["Length"] !== undefined))
		return --dim == 0 || isArray(obj[0],dim);
	return false;
}

const apply			= function(func, args, maxArgsNum = 1){
    let ct;
	if(isArray(args))
		if((ct = count(args)) > maxArgsNum)
			return each(args, item => apply(func,item,maxArgsNum));
		else return func.apply(null,args);
	else return func(args);
}

const between			= function(){
	for(const val of arguments)
		if(isSet(val)) return val;
	return arguments[arguments.length - 1];
}

const expand         	= function*(){
	for(const val of arguments)
		if(Array.isArray(val))
			for(const v of expand.apply(null,val))
				yield v;
		else if(isArray(val))
			for(const v of expand(each(val)))
				yield v;
		else yield val;
}

const array       		= function(arr) {
	if(isArray(arr))
		if(count(arr) === 1)
			return array(arr[0]);
		else if (Array.isArray(obj)) return arr;
		else return each(arr);
	else return [ arr ];
}

const each       		= function(arr, func = (o,i)=>o) 
{
	if(isArray(arr))
		// if(array.isarray(arr))
			// if(arr.length === 0) return [];
			// else return array.from(
				// (function* (){
					// let i = 0;
					// for(let item of arr)
						// yield func(item, i++);
				// })()
			// );
		// else 
		return Array.from(
				(function* (){
					let i = 0;
					for(const item of arr)
						yield func(item, i++);
				})()
			);
	else return [ func(arr,0) ];
}

const eachIn       		= function(arr, func = (o,i)=>o) 
{
	return Array.from(
			(function* (){
				let i = 0;
				for(const item in arr)
					yield func(item, i++);
			})()
		);
}

const where       		= function(arr, condition = (o,i)=>true, func = null) 
{
	if(func === null){
		if(isArray(arr))
			return Array.from(
					(function* (){
						let i = 0;
						for(const item of arr)
							if(condition(item, i++)) yield item;
					})()
				);
		else if(condition(arr,0)) return [ arr ];
	} else {
		if(isArray(arr))
			return Array.from(
					(function* (){
						let i = 0;
						for(const item of arr)
							if(condition(item, i)) yield func(item, i++);
							else i++;
					})()
				);
		else if(condition(arr,0)) return [ func(arr,0) ];
	}
	return [];
}

const any       		= function(arr, condition = (o,i)=>true, func = null) 
{
	if(func === null){
		if(isArray(arr))
			for(const item of arr)
				if(condition(item, i++)) 
					return item;
		else if(condition(arr,0)) return arr;
	} else {
		if(isArray(arr))
			for(const item of arr)
				if(condition(item, i))
					return func(item, i++);
				else i++;
		else if(condition(arr,0)) return func(arr,0);
	}
}

const select       		= function(arr, func = (o,i)=>o) 
{
	if(isArray(arr)) return each(arr,func);
	else return eachIn(arr,func);
}

const loop       		= function(i,len = null, func = i=>i) 
{
	if(len == null)
	{
		len = i;
		i = 0;
	}
	return Array.from(
			(function* (){
				for(i; i < len; i++)
					yield func(i);
			})()
		);
}

const first = function () {
	if (arguments.length == 0) return null;
	if (arguments.length == 1)
		if (isArray(arguments[0])) return arguments[0][0];
		else return arguments[0];
	if (arguments.length > 1) return arguments[0];
}

const last = function () {
	if (arguments.length == 0) return null;
	if (arguments.length == 1)
		if (isArray(arguments[0])) return arguments[0][arguments[0].length-1];
		else return arguments[0];
	if (arguments.length > 1) return arguments[0];
}

const count          	= function() 
{
    return each.apply(null,arguments).length;
}

const sort         		= function()
{
    if(arguments.length > 2) 
	{
		let func = arguments[2];
		return each.apply(null,arguments).sort((a, b)=> func(a) < func(b) ? -1 : func(a) > func(b));
	}
	return each.apply(null,arguments).sort((a, b)=> a < b ? -1 : a > b );
}

const distinct       	= function()
{
    if(arguments.length > 2) return each.apply(null,arguments).filter((v, i, a) => arguments[2](v, i, a));
	return each.apply(null,arguments).filter((v, i, a) => a.indexOf(v) === i);
}

const tryParseFloat    		= function(val) 
{
	return isEmpty(val)? 0 : between(parseFloat(val),val);
}

const tryParseInt    		= function(val) 
{
	return isEmpty(val)? 0 : between(parseInt(val),val);
}

const forceParseFloat    	= function(val,def = 0) 
{
	return parseFloat(val??def)??def;
}

const forceParseInt    		= function(val,def = 0) 
{
	return parseInt(val??def)??def;
}

const wait 				= function(milliseconds)
{
  const date = Date.now();
  let currentDate = null;
  do {
    currentDate = Date.now();
  } while (currentDate - date < milliseconds);
}

"Global Functions Library Installed Successfully"