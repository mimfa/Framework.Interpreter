const String                = (obj) => obj+"";

String.array                = (obj) => Array.from(String.enumerable(obj));

String.slice                = (obj, fromInd, count=-1) =>
{
    var i = 0;
    var res = [];
    for(const x of String.enumerable(obj)) 
        if(i<fromInd) i++;
        else if(count-- != 0){
            res.push(x);
            i++;
        }
    return res.join("");
}
String.enumerable       = function* (obj, nest = 0)
{
    if(obj === undefined || obj === null)
    {
        yield String(obj);
        return;
    }
    
    let objt = typeof obj;
    if(objt === 'string')
    {
        yield String(obj);
        return;
    }
    
    let indent = n => n >= 0? (n > 0? "\r\n" : "") + loop(0,n, i => "\t").join("") : "";
    let ind = indent(nest);
    let indp = indent(nest+1);
    if(Array.isArray(obj))
    {
        yield ind + "[";
        for(let x in each(obj, (x,i) => String.enumerable(x, nest + 1)))
            yield x+", ";
        yield "]";
        return;
    }
    try{
        if(objt === 'object' && isArray(obj))
        {
            yield ind + "[";
            for(let x in each(obj,
                (x,i) => String.enumerable(x, nest + 1)))
                yield x+", ";
            yield "]";
            return;
        }
        if(objt === 'object')
        {
            yield String(obj);
            return;
        }
    }
    catch{}
    
    yield String(obj);
}

"String Library Installed Successfully"