% ASP-LTLf Mapper
%
% INPUT:
% Sensor Fact:
% %intersection4Sensor_detected_car_NS(intersection4,objectIndex(Index),Value).
% %intersection4Sensor_detected_car_SN(intersection4,objectIndex(Index),Value).
% %intersection4Sensor_detected_car_EW(intersection4,objectIndex(Index),Value).
% %intersection4Sensor_detected_car_WE(intersection4,objectIndex(Index),Value).
%
% Self Index Fact:
% currentBrainID(ID).
%
% OUTPUT:
% ltlf_sensor(Output Variable)
%
% Output variable:
% ew_det, ns_det.

ltlf_sensor(ew_det) :- intersection4Sensor_detected_car_EW(intersection4,_,true).
ltlf_sensor(ew_det) :- intersection4Sensor_detected_car_WE(intersection4,_,true).
ltlf_sensor(ns_det) :- intersection4Sensor_detected_car_NS(intersection4,_,true).
ltlf_sensor(ns_det) :- intersection4Sensor_detected_car_SN(intersection4,_,true).