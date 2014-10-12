

$(document).ready(function () {

	// big dipper
	SetStar('big_dipper', 'big_dipper_alkaid');
	SetStar('big_dipper', 'big_dipper_mizar');
	SetStar('big_dipper', 'big_dipper_alioth');
	SetStar('big_dipper', 'big_dipper_megrez');
	SetStar('big_dipper', 'big_dipper_phecda');
	SetStar('big_dipper', 'big_dipper_merak');
	SetStar('big_dipper', 'big_dipper_dubhe');

	SetFindConstellation('big_dipper_find_1', 'big_dipper_find_1', 'big_dipper_find_1a');
	SetFindConstellation('big_dipper_find_2', 'big_dipper_find_2', 'big_dipper_find_2a');

	// little dipper
	SetFindConstellation('little_dipper_find', 'little_dipper_find_1', 'little_dipper_find_1a');

	// cassiopia
	SetStar('cassiopia', 'cassiopia_caph');
	SetStar('cassiopia', 'cassiopia_schedar');
	SetStar('cassiopia', 'cassiopia_gamma');
	SetStar('cassiopia', 'cassiopia_ruchbah');
	SetStar('cassiopia', 'cassiopia_epsilon');

	SetFindConstellation('cassiopia_find', 'cassiopia_find_1', 'cassiopia_find_1a');

	// cygnus
	SetStar('cygnus2', 'cygnus2_epsilon');
	SetStar('cygnus2', 'cygnus2_deneb');
	SetStar('cygnus2', 'cygnus2_sadr');
	SetStar('cygnus2', 'cygnus2_delta');
	SetStar('cygnus2', 'cygnus2_albireo');

	SetFindConstellation('cygnus_find', 'cygnus_find_1', 'cygnus_find_1a');

	// lyra
	SetStar('lyra', 'lyra_sulafat');
	SetStar('lyra', 'lyra_delta2');
	SetStar('lyra', 'lyra_sheliak');
	SetStar('lyra', 'lyra_zeta1');
	SetStar('lyra', 'lyra_vega');

	SetFindConstellation('lyra_find', 'lyra_find_1', 'lyra_find_1a');

	// sagittarius
	SetStar('sagittarius', 'sagittarius_milky');
	SetStar('sagittarius', 'sagittarius_ascella');
	SetStar('sagittarius', 'sagittarius_australis');
	SetStar('sagittarius', 'sagittarius_alnasi');
	SetStar('sagittarius', 'sagittarius_media');
	SetStar('sagittarius', 'sagittarius_borealis');
	SetStar('sagittarius', 'sagittarius_phi');
	SetStar('sagittarius', 'sagittarius_nunki');
	SetStar('sagittarius', 'sagittarius_tau');

	SetFindConstellation('sagittarius_find', 'sagittarius_find_1', 'sagittarius_find_1a');

	// hercules
	SetStar('hercules', 'hercules_theta');
	SetStar('hercules', 'hercules_pi');
	SetStar('hercules', 'hercules_epsilon');
	SetStar('hercules', 'hercules_delta');
	SetStar('hercules', 'hercules_kornephoros');
	SetStar('hercules', 'hercules_zeta');
	SetStar('hercules', 'hercules_eta');
	SetStar('hercules', 'hercules_sigma');
	SetStar('hercules', 'hercules_phi');

	SetFindConstellation('hercules_find', 'hercules_find_1', 'hercules_find_1a');

	// leo
	SetStar('leo', 'leo_zosma');
	SetStar('leo', 'leo_denebola');
	SetStar('leo', 'leo_chertan');
	SetStar('leo', 'leo_regulus');
	SetStar('leo', 'leo_eta');
	SetStar('leo', 'leo_algieba');
	SetStar('leo', 'leo_adhafera');
	SetStar('leo', 'leo_rasalas');
	SetStar('leo', 'leo_epsilon');

	SetFindConstellation('leo_find', 'leo_find_1', 'leo_find_1a');

	// orion
	SetStar('orion', 'orion_saiph');
	SetStar('orion', 'orion_rigel');
	SetStar('orion', 'orion_alnitak');
	SetStar('orion', 'orion_alnilam');
	SetStar('orion', 'orion_mintaka');
	SetStar('orion', 'orion_bellatrix');
	SetStar('orion', 'orion_meissa');
	SetStar('orion', 'orion_betelgeuse');

	SetFindConstellation('orion_find', 'orion_find_1', 'orion_find_1a');

});

function SetStar(mainPicture, nameOfStar) {
	$('#' + nameOfStar).mouseover(function () {
		$('#' + mainPicture).attr('src', '/Content/AstronomyImages/Constellations/' + nameOfStar + '.jpg');
	});

	$('#' + nameOfStar).mouseout(function () {
		$('#' + mainPicture).attr('src', '/Content/AstronomyImages/Constellations/' + mainPicture + '.jpg');
	});
}

function SetFindConstellation(className, mainPicture, nameOfConstellation) {

	$('.' + className).mouseover(function () {
		$('#' + mainPicture).attr('src', '/Content/AstronomyImages/Constellations/' + nameOfConstellation + '.jpg');
	});

	$('.' + className).mouseout(function () {
		$('#' + mainPicture).attr('src', '/Content/AstronomyImages/Constellations/' + mainPicture + '.jpg');
	});
}