import { ScanPersonResponseBase } from "./scan.person.response.base";

export class ScanPersonResultResponse<T> extends ScanPersonResponseBase  {

    constructor(
        public result: T,
        public override isSuccess?: boolean,
        public override error?: string) {
        super(isSuccess, error);
    }
    }