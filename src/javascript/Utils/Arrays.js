// This file contains utility functions for working with arrays.

class Arrays{
    static toTypedArray(array, type){
        switch(type){
            case "Single":
                return new Float32Array(array);
            case "Double":
                return new Float64Array(array);
            case "SByte":
                return new Int8Array(array);
            case "Int16":
                return new Int16Array(array);
            case "Int32":
                return new Int32Array(array);
            case "Byte":
                return new Uint8Array(array);
            case "Uint16":
                return new Uint16Array(array);
            case "Uint32":
                return new Uint32Array(array);
            default:
                throw new Error(`Unsupported type: ${type}`);
        }
    }
}

export default Arrays;