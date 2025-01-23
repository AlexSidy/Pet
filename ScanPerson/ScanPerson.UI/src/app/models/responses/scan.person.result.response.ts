import { ScanPersonResponse } from "./scan.person.response";

export class ScanPersonResultResponse extends ScanPersonResponse  {

    constructor(
        public Result: any,
        public override isSuccess?: boolean,
        public override error?: string) {
        super(isSuccess, error);
    }
    }