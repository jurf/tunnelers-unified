//
//  ILife.cs is part of Tunnelers: Unified
//  <https://github.com/VacuumGames/tunnelers-unified/>
//
//  Copyright (c) 2015 Juraj Fiala <doctorjellyface@riseup.net>
//
//  Tunnelers: Unified is free software: you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
//
//  Tunnelers: Unified is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU General Public License for more details.
//
//  You should have received a copy of the GNU General Public License
//  along with Tunnelers: Unified.  If not, see <http://www.gnu.org/licenses/>.
//

public interface ILife {

	// Stuff like power-ups needs to be able to
	// change the shield and energy
	float Energy {
		get;
		set;
	}
	
	float Shield {
		get;
		set;
	}

	// Weapons need to know if they can shoot
	bool HaveEnergy {
		get;
	}

	// Other tanks need to use this so that
	// they know if they've killed or not
	void Damage (float amount, out bool killed);
	
}

