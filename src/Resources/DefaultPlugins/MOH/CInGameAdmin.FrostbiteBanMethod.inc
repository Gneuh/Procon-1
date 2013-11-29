			public string FrostbiteBanMethod {
				get {
					string frostbiteBanMethod = "none";
				
					if (this.BanMethod == BanTypes.FrostbiteName) {
						frostbiteBanMethod = "name";
					}
					else if (this.BanMethod == BanTypes.FrostbiteEaGuid) {
						frostbiteBanMethod = "guid";
					}

					return frostbiteBanMethod;
				}
			}