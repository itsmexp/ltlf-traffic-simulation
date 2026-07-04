% ASP-LTLf Mapper
%
% INPUT:
% Sensor Fact:
% %intersection7Sensor_detected_car_NS(intersection7,objectIndex(Index),Value).
% %intersection7Sensor_detected_car_SN(intersection7,objectIndex(Index),Value).
% %intersection7Sensor_detected_car_EW(intersection7,objectIndex(Index),Value).
% %intersection7Sensor_detected_car_WE(intersection7,objectIndex(Index),Value).
%
% Self Index Fact:
% currentBrainID(ID).
%
% OUTPUT:
% ltlf_sensor(Output Variable)
%
% Output variable:
% ew_det, ns_det.

ltlf_sensor(ew_det) :- intersection7Sensor_detected_car_EW(intersection7,_,true).
ltlf_sensor(ew_det) :- intersection7Sensor_detected_car_WE(intersection7,_,true).
ltlf_sensor(ns_det) :- intersection7Sensor_detected_car_NS(intersection7,_,true).
ltlf_sensor(ns_det) :- intersection7Sensor_detected_car_SN(intersection7,_,true).