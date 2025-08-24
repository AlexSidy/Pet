export class PersonInfoItem {
    constructor(
        public id?: number,
        public name?: string,
        public mail?: string,
        public location?: LocationItem
      ) { }
}

export class LocationItem {
		countryName: string = '';
		currentRegion: string = '';
		registrationOkrug: string = '';
		registrationCapital: string = '';
		operatorName: string = '';
		operatorCity: string = ''
    constructor(
    ) { }
}