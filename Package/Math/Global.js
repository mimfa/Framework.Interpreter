const isEven         = function() 
{
	return Math.isEven(Array.forceParseInt.apply(null,arguments));
}

const isOdd         = function() {    
	return Math.isOdd(Array.forceParseInt.apply(null,arguments));
}


const frequent       = function() 
{
    return Array.frequent.apply(null,arguments);
}

const sum            = function() 
{
    return Math.sum(Array.forceParseFloat.apply(null,arguments));
}

const mean           = function() 
{
    return Math.mean(Array.forceParseFloat.apply(null,arguments));
}

const median         = function() 
{
    return Math.median(Array.forceParseFloat.apply(null,arguments));
}

const mode           = function() 
{
    return Math.mode.apply(null,arguments);
}

const range           = function() 
{
    return Math.range(Array.forceParseFloat.apply(null,arguments));
}

const minimum        = function() 
{
    return Math.minimum(Array.forceParseFloat.apply(null,arguments));
}

const maximum        = function() 
{
    return Math.maximum(Array.forceParseFloat.apply(null,arguments));
}

const min        	= function() 
{
    return Math.min.apply(null,Array.forceParseFloat.apply(null,arguments));
}

const max        	= function() 
{
    return Math.max.apply(null,Array.forceParseFloat.apply(null,arguments));
}

const variance     	= function() 
{
    return Math.variance(Array.forceParseFloat.apply(null,arguments));
}

const std           = function() 
{
    return Math.std(Array.forceParseFloat.apply(null,arguments));
}

const radical      = function() { return apply(Math.radical,Array.forceParseFloat.apply(null,arguments),2); }

const sqrt      	= function() { return apply(Math.sqrt,Array.forceParseFloat.apply(null,arguments),1); }

const pow          = function() { return apply(Math.pow,Array.forceParseFloat.apply(null,arguments),2); }

const power          = function() { return apply(Math.power,Array.forceParseFloat.apply(null,arguments),2); }

const ln           = function() { return apply(Math.log,Array.forceParseFloat.apply(null,arguments),1); }

const log          = function() { return apply(Math.logarithm,Array.forceParseFloat.apply(null,arguments),2); }

const sin           = function() { return apply(Math.sin,Array.forceParseFloat.apply(null,arguments),1); };

const cos           = function() { return apply(Math.cos,Array.forceParseFloat.apply(null,arguments),1); };

const tan           = function() { return apply(Math.tan,Array.forceParseFloat.apply(null,arguments),1); };

const cot           = function() { return apply(Math.cot,Array.forceParseFloat.apply(null,arguments),1); };

const round           = function() { return apply(Math.round,Array.forceParseFloat.apply(null,arguments),1); };

const ceil           = function() { return apply(Math.ceil,Array.forceParseFloat.apply(null,arguments),1); };

const floor           = function() { return apply(Math.floor,Array.forceParseFloat.apply(null,arguments),1); };

const trunc           = function() { return apply(Math.trunc,Array.forceParseFloat.apply(null,arguments),1); };

const sign           = function() { return apply(Math.sign,Array.forceParseFloat.apply(null,arguments),1); };

const abs           = function() { return apply(Math.abs,Array.forceParseFloat.apply(null,arguments),1); };

const random           = function() { return apply(Math.random,Array.forceParseFloat.apply(null,arguments),2); };

const decimals          = function() { return apply(Math.decimals,Array.forceParseFloat.apply(null,arguments),2); };

"Global Math Library Installed Successfully"