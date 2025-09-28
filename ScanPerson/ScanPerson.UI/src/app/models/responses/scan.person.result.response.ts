import { ScanPersonResponseBase } from "./scan.person.response.base";

export class ScanPersonResultResponse<T> extends ScanPersonResponseBase  {

    constructor(
        public override result: T,
        public override isSuccess?: boolean,
        public override error?: string) {
        super(result, isSuccess, error);
    }
    }