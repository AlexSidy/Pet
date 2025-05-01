import { ScanPersonResponse } from "./scan.person.response";

export class ScanPersonResultResponse extends ScanPersonResponse  {

    constructor(
        public result: any,
        public override isSuccess?: boolean,
        public override error?: string) {
        super(isSuccess, error);
    }
    }