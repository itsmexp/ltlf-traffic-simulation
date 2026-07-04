% ASP-LTLf Mapper
%
% INPUT:
% Sensor Fact:
% %intersection5Sensor_detected_car_NS(intersection5,objectIndex(Index),Value).
% %intersection5Sensor_detected_car_SN(intersection5,objectIndex(Index),Value).
% %intersection5Sensor_detected_car_EW(intersection5,objectIndex(Index),Value).
% %intersection5Sensor_detected_car_WE(intersection5,objectIndex(Index),Value).
%
% Self Index Fact:
% currentBrainID(ID).
%
% OUTPUT:
% ltlf_sensor(Output Variable)
%
% Output variable:
% ew_det, ns_det.

ltlf_sensor(ew_det) :- intersection5Sensor_detected_car_EW(intersection5,_,true).
ltlf_sensor(ew_det) :- intersection5Sensor_detected_car_WE(intersection5,_,true).
ltlf_sensor(ns_det) :- intersection5Sensor_detected_car_NS(intersection5,_,true).
ltlf_sensor(ns_det) :- intersection5Sensor_detected_car_SN(intersection5,_,true).